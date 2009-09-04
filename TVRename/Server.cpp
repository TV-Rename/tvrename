#include "StdAfx.h"
//#include "Server.h"
//#include "TVDoc.h"

/*
using namespace System;
using namespace System::IO;
using namespace System::Net;
using namespace System::Net::Sockets;
using namespace System::Text;
using namespace System::Threading;

namespace TVRename {
	String ^TVRenameServer::Err()
		{
			return "<!DOCTYPE HTML PUBLIC \"-//IETF//DTD HTML 2.0//EN\"><html><head><title>404 Not Found</title></head><body><h1>Not Found</h1>"
				"<p>The requested URL was not found on this server.</p>"
				"<hr><p>TVRename on localhost " + DateTime::Now.ToString("g") + "</p></body></html>";
		}

		String ^TVRenameServer::ProcessLine(String ^line, TVDoc ^doc)
		{
			array<String^> ^parts = line->Split(' ');
			if (parts->Length != 3)
				return "";
			else if (parts[0]->ToUpper() != "GET")
				return "";
			else if (parts[1]->EndsWith("/upcoming.xml"))
			{
				MemoryStream ^ms = gcnew MemoryStream();
				doc->GenerateUpcomingXML(ms, doc->NextNShows(doc->Settings->ExportRSSMaxShows, doc->Settings->ExportRSSMaxDays));
				return System::Text::Encoding::ASCII->GetString(ms->ToArray());
			}
			else
				return Err();

		}

		TVRenameServer::TVRenameServer(TVDoc ^doc)
		{
			return; // disabled for now.

			for (;;)
			{
				try
				{
					// Set the TcpListener on port 13000.
					Int32 port = 8085;
					IPAddress^ localAddr = IPAddress::Parse( "127.0.0.1" );

					TcpListener^ server = gcnew TcpListener( localAddr,port );

					// Start listening for client requests.
					server->Start();

					// Buffer for reading data
					array<Byte>^bytes = gcnew array<Byte>(256);
					String^ data = nullptr;

					// Enter the listening loop.
					while ( true )
					{
						// Perform a blocking call to accept requests.
						// You could also user server.AcceptSocket() here.
						TcpClient^ client = server->AcceptTcpClient();
						data = nullptr;

						// Get a stream Object* for reading and writing
						NetworkStream^ stream = client->GetStream();
						Int32 i;

						// Loop to receive all the data sent by the client.
						String ^line = "";
						String ^getLine = "";
						bool done = false;
						while (! done && ( i = stream->Read( bytes, 0, bytes->Length ) ))
						{
							// Translate data bytes to a ASCII String*.
							data = System::Text::Encoding::ASCII->GetString( bytes, 0, i );

							for (int p=0;p<data->Length;p++)
							{
								wchar_t c = data[p];
								if ((c == 0x0d) || (c == 0x0a))
								{
									if ((c == 0x0d) && ((p+1)<data->Length) && (data[p+1] == 0x0a))
										p++; // skip LF following CR

									if (line->Length)
									{
										if (line->Substring(0,4)->ToUpper() == "GET ")
											getLine = line;
									}
									else
									{
										if (!String::IsNullOrEmpty(getLine)) // this line is blank, and we have a GET line saved
										{
											
										String ^res = ProcessLine(getLine, doc);
										array<Byte>^msg = System::Text::Encoding::ASCII->GetBytes( res );
										stream->Write( msg, 0, msg->Length );
										getLine = "";
										done = true;
										break;
										}
									}

									line = "";
								}
								else
									line += c;
							}

						}
						client->Close(); // Shutdown and end connection


						
					}
				} // try
				catch ( SocketException^e) 
				{
					System::Windows::Forms::MessageBox::Show(e->ToString());
				}
				catch (ThreadAbortException ^) 
				{
					// time to stop
									
					return;  // we're outta here!

				}
			} // loop forever
		}

} // namespace
*/
