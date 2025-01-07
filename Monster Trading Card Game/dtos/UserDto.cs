namespace mtcg{
    internal class UserDto{
        public string? username { get; set; }
        public string? new_username { get; set; }
        public string? password { get; set; }
        public string? password_repeat { get; set; }

        public UserDto(string? username, string? password, string? new_username = null, string? password_repeat = null){
            this.username = username;
            this.new_username = new_username;
            this.password = password;
            this.password_repeat = password_repeat;
        }
    }
}