using System;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using Alphaleonis.Win32.Filesystem;

namespace TVRename.Core.Actions
{
    public class FileAction : IAction
    {
        private readonly FileInfo source;
        private readonly FileInfo dest;

        public string Type => this.Operation.ToString();

        public string Produces => this.dest.FullName;

        public FileOperation Operation { get; }

        public FileAction(FileInfo source, FileInfo dest, FileOperation operation)
        {
            this.source = source;
            this.dest = dest;
            this.Operation = operation;
        }

        public async Task Run(CancellationToken ct)
        {
            await Task.Factory.StartNew(() =>
            {
                FileSecurity security = null;

                try
                {
                    security = this.source.GetAccessControl();
                }
                catch
                {
                    // ignored
                }

                CopyMoveResult result;

                switch (this.Operation)
                {
                    case FileOperation.Copy:
                        result = File.Copy(this.source.FullName, this.dest.FullName, CopyOptions.None, true, null, null);
                        break;

                    case FileOperation.Move:
                        result = File.Move(this.source.FullName, this.dest.FullName, MoveOptions.CopyAllowed | MoveOptions.ReplaceExisting, null, null);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (result.ErrorCode != 0) throw new Exception(result.ErrorMessage);

                try
                {
                    if (security != null) this.dest.SetAccessControl(security);
                }
                catch
                {
                    // ignored
                }
            }, ct);
        }

        public enum FileOperation
        {
            Copy,
            Move,
            Rename, // TODO
            Delete // TODO
        }
    }
}
