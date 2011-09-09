HARDEN
======


Your domain model is weak. It's a pansy. It needs to be *Hardened*.

Harden takes your objects and wraps them in a super hard layer, preventing code from being called when it's not meant to be called, in ways it's not meant to be called.

Heres an example of a hardened class:

    public class User
    {
        public virtual void Ban() { ... }

        public bool AllowBan()
        {
            return Context.CurrentUser.IsAdmin; //only admins can Ban users
        }
        
        public virtual string Email { get; set; }

        public bool AllowEmail()
        {
            return Context.CurrentUser.IsAdmin || Context.CurrentUser == this;
            //only admins can see or change other peoples emails
        }                
    }


