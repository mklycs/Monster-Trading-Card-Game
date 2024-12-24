using System;

namespace mtcg{
    internal class Validator{
        public Validator(){ }
        public int checkInt(int min, int max, string msg){ // das muss auch noch überarbeitet werden
            while(true){
                Console.Write(msg);
                try{
                    int input = Convert.ToInt32(Console.ReadLine());
                    if(input >= min && input <= max)
                        return input;

                    Console.Write("Invalid input.\n\n");
                }
                catch(FormatException){
                    Console.Write("Invalid input.\n\n");
                }catch(Exception exception){
                    Console.WriteLine($"Something went wrong: {exception.Message}");
                }
            }
        }

        public string? checkStr(int min, string? str){
            if(str?.Length >= min) return str;
            else return null;
        }
    }
}