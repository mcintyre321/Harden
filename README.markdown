HARDEN
======

For .NET

Your domain model is weak. It's a pansy. It needs to be *Hardened*.

Harden takes your objects and wraps them in a super hard layer, preventing code from being called when it's not meant to be called, in ways it's not meant to be called.

Heres an example of a class ready to be hardened:

    public class User
    {
        public virtual void Ban() { ... } //virtual so we can proxy it

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

Now we harden it:

    var user = Hardener.Harden(user);
    user.Ban; //You better be an admin, or you just got yourself a HardenException


The Why
-------

Some of you might be asking why you need this. After all, the only people who can access against your model are you and other developers at your company...

STOP. RIGHT. THERE.

Like it or not, objects end up with internal state machines, with methods that should be called in certain ways.
Whether it's someone else at your company, someone who used to work there or someone who will, at some point your objects are going to be misused, maltreated 

and a new bug is going to have to be found and raised and fixed. Invalid calls to Hardened objects don't make it into the trunk.

Of course, you don't need Harden to harden an object, but it helps.


Validation
----------

OH yeah it validates method parameters too, so your

    public virtual void DoSomething(DateTime a, DateTime b, string c, int d){ ... }
    public IEnumerable<Error> ValidateDoSomething(DateTime a, DateTime b, string c) //d omitted as it doesn't need validation
    {
        if (a > b) yield return new Error("a", "First date must be on or before second");
        if (c.IsRudeWord()) yield return new Error("c", "Naughty naughty!");
    }


and properties

    public virtual string Name { get; set; }
    public IEnumerable<Error> ValidateName(string value)
    {
        if (string.IsNullOrEmpty(value)) yield return new Error("c", "Name must be entered");
        if ((value ?? "").Length < 4) yield return new Error("c", "A REAL name must be entered!");      
    }

Call with bad data and you get a single ValidationException with all the errors in it.

Syntax
------

Pretty simple, AllowX methods must return bool? (null means that this method isn't sure whether to allow or deny), ValidateX methods return IEnumerable<Error>

If you only want to do a property getter or setter, you can write AllowGetX or AllowSetY

If you want to put an allow method for all properties, you can write:

> public bool? Allow(MethodInfo calledMethodInfo)



To install Harden use the nuget browser or from the package manager console in VS:    
    
    PM> Install-Package Harden
    
Advanced usage
--------------

I hooked the HardenInterceptor up to my IOC container, now all my entities as as hard as nails. I hardened my API, and now I don't worry about script kiddies screwing with Joe Public's data.
    
Remember HARD CODE IS GOOD CODE
    
