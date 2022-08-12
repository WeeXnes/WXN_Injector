using System.Drawing;

namespace WXN_Injector.core
{
    public class ProcessInfoTemplate
    {
        public string ProcessName { get; set; }
        public int ProcessID { get; set; }
        
        public ProcessInfoTemplate(string _ProcessName, int _ProcessID)
        {
            this.ProcessName = _ProcessName;
            this.ProcessID = _ProcessID;
        }
    }
}