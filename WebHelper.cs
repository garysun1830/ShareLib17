using System;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;

namespace ShareLib17
{

    public class WebExceptionTimeout : Exception
    {
        public WebExceptionTimeout() : base("") { }
    }

    public class WebExceptionCancel : Exception
    {
        public WebExceptionCancel() : base("") { }
    }

    public class WebExceptionForbidden : Exception
    {
        public WebExceptionForbidden() : base("") { }
        public WebExceptionForbidden(string message) : base(message) { }
    }

    public class WebExceptionContent : Exception
    {
        public WebExceptionContent() : base("") { }
        public WebExceptionContent(string message) : base(message) { }
    }

    public interface IUserInteract
    {
        bool UserCancelTask();
        void ProgressChanged(long received, int percent);
    }

}
