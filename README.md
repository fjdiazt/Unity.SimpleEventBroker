Unity.SimpleEventBroker
=======================

Unity SimpleEventBrokerExtension

[Example usage](http://msdn.microsoft.com/en-us/library/dn178462(v=pandp.30).aspx#sec8)

    class Publisher
    {
        [Publishes("CustomEvent")]
        public event EventHandler RaiseCustomEvent;

        public void DoSomething()
        {
            OnRaiseCustomEvent();
        }

        protected virtual void OnRaiseCustomEvent()
        {
            EventHandler handler = RaiseCustomEvent;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }

    class Subscriber
    {
        private string id;

        public Subscriber(string ID)
        {
            id = ID;
        }

        [SubscribesTo("CustomEvent")]
        public void HandleCustomEvent(object sender, EventArgs e)
        {
            Console.WriteLine("Subscriber {0} received this message at: {1}", id, DateTime.Now);
        }
    }
    
    IUnityContainer container = new UnityContainer();

    container.AddNewExtension<SimpleEventBrokerExtension>()
             .RegisterType<Publisher>()
             .RegisterType<Subscriber>(new InjectionConstructor("default"));

    var sub1 = container.Resolve<Subscriber>(new ParameterOverride("ID", "sub1"));
    var sub2 = container.Resolve<Subscriber>(new ParameterOverride("ID", "sub2"));

    // Call the method
    pub.DoSomething();

The MIT License (MIT)
=============

Copyright (c) 2013 Sander Struijk

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
