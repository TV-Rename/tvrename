// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// TVRename code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//
// This file is under a "free for any use" license.
//
// Original code is from: http://designingefficientsoftware.wordpress.com/2011/03/03/efficient-file-io-from-csharp/
//
// Modified and streamlined by Steve, for TVRename use, specifically to have separate read/write file handles

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Win32FileIO
{
    unsafe public class WinFileIO : IDisposable
    {
        // This class provides the capability to utilize the ReadFile and Writefile windows IO functions.  These functions
        // are the most efficient way to perform file I/O from C# or even C++.  The constructor with the buffer and buffer
        // size should usually be called to init this class.  PinBuffer is provided as an alternative.  The reason for this
        // is because a pointer needs to be obtained before the ReadFile or WriteFile functions are called.
        //
        // Error handling - In each public function of this class where an error can occur, an ApplicationException is
        // thrown with the Win32Exception message info if an error is detected.  If no exception is thrown, then a normal
        // return is considered success.
        // 
        // This code is not thread safe.  Thread control primitives need to be added if running this in a multi-threaded
        // environment.
        //
        // The recommended and fastest function for reading from a file is to call the ReadBlocks method.
        // The recommended and fastest function for writing to a file is to call the WriteBlocks method.
        //
        // License and disclaimer:
        // This software is free to use by any individual or entity for any endeavor for profit or not.
        // Even though this code has been tested and automated unit tests are provided, there is no gaurantee that
        // it will run correctly with your system or environment.  I am not responsible for any failure and you agree
        // that you accept any and all risk for using this software.
        //
        //
        // Written by Robert G. Bryan in Feb, 2011.
        //
        // Constants required to handle file I/O:
        private const uint GENERIC_READ = 0x80000000;
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint FILE_SHARE_READ = 0x00000001;

        private const uint OPEN_EXISTING = 3;
        private const uint CREATE_ALWAYS = 2;
        private const int BlockSize = 65536;
        //
        private GCHandle gchBuf;            // Handle to GCHandle object used to pin the I/O buffer in memory.
        private System.IntPtr pHandleRead;
        private System.IntPtr pHandleWrite;
        private void* pBuffer;              // Pointer to the buffer used to perform I/O.

        // Define the Windows system functions that are called by this class via COM Interop:
        [System.Runtime.InteropServices.DllImport("kernel32", SetLastError = true)]
        static extern unsafe System.IntPtr CreateFile
        (
             string FileName,          // file name
             uint DesiredAccess,       // access mode
             uint ShareMode,           // share mode
             uint SecurityAttributes,  // Security Attributes
             uint CreationDisposition, // how to create
             uint FlagsAndAttributes,  // file attributes
             int hTemplateFile         // handle to template file
        );

        [System.Runtime.InteropServices.DllImport("kernel32", SetLastError = true)]
        static extern unsafe bool ReadFile
        (
             System.IntPtr hFile,      // handle to file
             void* pBuffer,            // data buffer
             int NumberOfBytesToRead,  // number of bytes to read
             int* pNumberOfBytesRead,  // number of bytes read
             int Overlapped            // overlapped buffer which is used for async I/O.  Not used here.
        );

        [System.Runtime.InteropServices.DllImport("kernel32", SetLastError = true)]
        static extern unsafe bool WriteFile
        (
            IntPtr handle,					   // handle to file
            void* pBuffer,             // data buffer
            int NumberOfBytesToWrite,	 // Number of bytes to write.
            int* pNumberOfBytesWritten,// Number of bytes that were written..
            int Overlapped					   // Overlapped buffer which is used for async I/O.  Not used here.
        );

        [System.Runtime.InteropServices.DllImport("kernel32", SetLastError = true)]
        static extern unsafe bool CloseHandle
        (
             System.IntPtr hObject     // handle to object
        );

        public WinFileIO(Array Buffer)
        {
            // This constructor is provided so that the buffer can be pinned in memory.
            // Cleanup must be called in order to unpin the buffer.
            PinBuffer(Buffer);
            pHandleRead = IntPtr.Zero;
            pHandleWrite = IntPtr.Zero;
        }

        protected void Dispose(bool disposing)
        {
            // This function frees up the unmanaged resources of this class.
            Close();
            UnpinBuffer();
        }

        public void Dispose()
        {
            // This method should be called to clean everything up.
            Dispose(true);
            // Tell the GC not to finalize since clean up has already been done.
            GC.SuppressFinalize(this);
        }

        ~WinFileIO()
        {
            // Finalizer gets called by the garbage collector if the user did not call Dispose.
            Dispose(false);
        }

        private void PinBuffer(Array Buffer)
        {
            // This function must be called to pin the buffer in memory before any file I/O is done.
            // This shows how to pin a buffer in memory for an extended period of time without using
            // the "Fixed" statement.  Pinning a buffer in memory can take some cycles, so this technique
            // is helpful when doing quite a bit of file I/O.
            //
            // Make sure we don't leak memory if this function was called before and the UnPinBuffer was not called.
            UnpinBuffer();
            gchBuf = GCHandle.Alloc(Buffer, GCHandleType.Pinned);
            IntPtr pAddr = Marshal.UnsafeAddrOfPinnedArrayElement(Buffer, 0);
            // pBuffer is the pointer used for all of the I/O functions in this class.
            pBuffer = (void*)pAddr.ToPointer();
        }

        public void UnpinBuffer()
        {
            // This function unpins the buffer and needs to be called before a new buffer is pinned or
            // when disposing of this object.  It does not need to be called directly since the code in Dispose
            // or PinBuffer will automatically call this function.
            if (gchBuf.IsAllocated)
                gchBuf.Free();
        }

        public void OpenForReading(string FileName)
        {
            // This function uses the Windows API CreateFile function to open an existing file.
            // A return value of true indicates success.
            Close(true, false);
            pHandleRead = CreateFile(FileName, GENERIC_READ, FILE_SHARE_READ, 0, OPEN_EXISTING, 0, 0);
            if (pHandleRead == System.IntPtr.Zero)
            {
                Win32Exception WE = new Win32Exception();
                ApplicationException AE = new ApplicationException("WinFileIO:OpenForReading - Could not open file " +
                  FileName + " - " + WE.Message);
                throw AE;
            }
        }

        public void OpenForWriting(string FileName)
        {
            // This function uses the Windows API CreateFile function to open an existing file.
            // If the file exists, it will be overwritten.
            Close(false, true);
            pHandleWrite = CreateFile(FileName, GENERIC_WRITE, 0, 0, CREATE_ALWAYS, 0, 0);
            if (pHandleWrite == System.IntPtr.Zero)
            {
                Win32Exception WE = new Win32Exception();
                ApplicationException AE = new ApplicationException("WinFileIO:OpenForWriting - Could not open file " +
                    FileName + " - " + WE.Message);
                throw AE;
            }
        }
        /*
        public int Read(int BytesToRead)
        {
            // This function reads in a file up to BytesToRead using the Windows API function ReadFile.  The return value
            // is the number of bytes read.
            int BytesRead = 0;
            if (!ReadFile(pHandleRead, pBuffer, BytesToRead, &BytesRead, 0))
            {
                Win32Exception WE = new Win32Exception();
                ApplicationException AE = new ApplicationException("WinFileIO:Read - Error occurred reading a file. - " +
                    WE.Message);
                throw AE;
            }
            return BytesRead;
        }

        public int ReadUntilEOF()
        {
            // This function reads in chunks at a time instead of the entire file.  Make sure the file is <= 2GB.
            // Also, if the buffer is not large enough to read the file, then an ApplicationException will be thrown.
            // No check is made to see if the buffer is large enough to hold the file.  If this is needed, then
            // use the ReadBlocks function below.
            int BytesReadInBlock = 0, BytesRead = 0;
            byte* pBuf = (byte*)pBuffer;
            // Do until there are no more bytes to read or the buffer is full.
            for (; ; )
            {
                if (!ReadFile(pHandleWrite, pBuf, BlockSize, &BytesReadInBlock, 0))
                {
                    // This is an error condition.  The error msg can be obtained by creating a Win32Exception and
                    // using the Message property to obtain a description of the error that was encountered.
                    Win32Exception WE = new Win32Exception();
                    ApplicationException AE = new ApplicationException("WinFileIO:ReadUntilEOF - Error occurred reading a file. - "
                        + WE.Message);
                    throw AE;
                }
                if (BytesReadInBlock == 0)
                    break;
                BytesRead += BytesReadInBlock;
                pBuf += BytesReadInBlock;
            }
            return BytesRead;
        }
        */
        public int ReadBlocks(int BytesToRead)
        {
            // This function reads a total of BytesToRead at a time.  There is a limit of 2gb per call.
            int BytesReadInBlock = 0, BytesRead = 0;
            byte* pBuf = (byte*)pBuffer;
            // Do until there are no more bytes to read or the buffer is full.
            do
            {
                int BlockByteSize = Math.Min(BlockSize, BytesToRead - BytesRead);
                if (!ReadFile(pHandleRead, pBuf, BlockByteSize, &BytesReadInBlock, 0))
                {
                    Win32Exception WE = new Win32Exception();
                    ApplicationException AE = new ApplicationException("WinFileIO:ReadBytes - Error occurred reading a file. - "
                        + WE.Message);
                    throw AE;
                }
                if (BytesReadInBlock == 0)
                    break;
                BytesRead += BytesReadInBlock;
                pBuf += BytesReadInBlock;
            } while (BytesRead < BytesToRead);
            return BytesRead;
        }
        /*
        public int Write(int BytesToWrite)
        {
            // Writes out the file in one swoop using the Windows WriteFile function.
            int NumberOfBytesWritten;
            if (!WriteFile(pHandleWrite, pBuffer, BytesToWrite, &NumberOfBytesWritten, 0))
            {
                Win32Exception WE = new Win32Exception();
                ApplicationException AE = new ApplicationException("WinFileIO:Write - Error occurred writing a file. - " +
                    WE.Message);
                throw AE;
            }
            return NumberOfBytesWritten;
        }
        */
        public int WriteBlocks(int NumBytesToWrite)
        {
            // This function writes out chunks at a time instead of the entire file.  This is the fastest write function,
            // perhaps because the block size is an even multiple of the sector size.
            int BytesWritten = 0;
            int BytesOutput = 0;
            byte* pBuf = (byte*)pBuffer;
            int RemainingBytes = NumBytesToWrite;
            // Do until there are no more bytes to write.
            do
            {
                int BytesToWrite = Math.Min(RemainingBytes, BlockSize);
                if (!WriteFile(pHandleWrite, pBuf, BytesToWrite, &BytesWritten, 0))
                {
                    // This is an error condition.  The error msg can be obtained by creating a Win32Exception and
                    // using the Message property to obtain a description of the error that was encountered.
                    Win32Exception WE = new Win32Exception();
                    ApplicationException AE = new ApplicationException("WinFileIO:WriteBlocks - Error occurred writing a file. - "
                        + WE.Message);
                    throw AE;
                }
                pBuf += BytesToWrite;
                BytesOutput += BytesToWrite;
                RemainingBytes -= BytesToWrite;
            } while (RemainingBytes > 0);
            return BytesOutput;
        }

        public bool Close()
        {
            return Close(true, true);
        }

        public bool Close(bool read, bool write)
        {
            // This function closes the file handle.
            bool Success = true;
            if (write && pHandleWrite != IntPtr.Zero)
            {
                Success = CloseHandle(pHandleWrite) && Success;
                pHandleWrite = IntPtr.Zero;
            }
            if (read && pHandleRead != IntPtr.Zero)
            {
                Success = CloseHandle(pHandleRead) && Success;
                pHandleRead = IntPtr.Zero;
            }
            return Success;
        }
    }
}
