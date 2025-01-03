using System;
using System.Collections.Generic;

namespace mtcg{
    internal class Program{
        static void Main(string[] args){
            Server server = new Server();
            server.start();
        }
    }
}