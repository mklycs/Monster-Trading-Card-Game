using System;

namespace mtcg{
    public abstract class A_Card{
        public string? name { get; set; }
        public int damage { get; set; }
        public char cardType { get; set; }
        public string? elementType { get; set; }  // "fire", "water", "normal"
    }
}