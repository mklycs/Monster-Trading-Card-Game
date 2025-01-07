namespace mtcg{
    internal class Validator{
        public Validator(){ }

        public string? checkStr(int min, string? str){
            if(str?.Length >= min) return str;
            else return null;
        }
    }
}