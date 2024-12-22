using System;

namespace mtcg{
    internal class Status{
        public int statusCode { get; set; }
        public string message { get; set; }

        public Status(int statusCode = 500, string message = "something went wrong."){
            this.statusCode = statusCode;
            this.message = message;
        }
    }
}