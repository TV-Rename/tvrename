using System.ComponentModel;

namespace TVRename 
{ 
public interface ISplashForm : ISynchronizeInvoke 
{ 
void UpdateStatus(string status); 
void UpdateProgress(int progress); 
void UpdateInfo(string info); 
} 
}

