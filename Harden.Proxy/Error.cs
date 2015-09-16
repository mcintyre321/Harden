namespace Harden
{
    public class Error
    {
        public Error(string field, string message)
        {
            Field = field;
            Message = message;
        }

        public Error(string message)
        {
            Message = message;
        }


        public string Message { get; private set; }

        public string Field { get; internal set; }
       
        
        public override string ToString()
        {
            return !string.IsNullOrWhiteSpace(Field)
                ? string.Format("Field: {0}; Message: {1}", Field, Message)
                : "Message: " + Message;
        }
    }
}