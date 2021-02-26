using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace sharedrasil {
    public class Token {
        public string Type {get; set;}
        public string AccessToken {get; set;}
    }

    public class User {
        public string Username {get;set;}
        public string Email {get;set;}
        public Token AccessToken {get;set;}

        public async void GetCredentials() {
            string userPath = Path.Combine(Globals.LOCALREPO_PATH, "user.json");
            if(!File.Exists(userPath)) {
                Console.WriteLine("You don't have a user created.");
                Boolean wantsToCreate = CLIParser.AskYesOrNo("Would you like to create one now?");

                if(!wantsToCreate) {
                    Console.WriteLine("Unfortunately, Sharedrasil needs a local user in order to comunicate with Github");
                    return;
                } else {
                    LocalRepo localRepo = new LocalRepo();
                    await localRepo.AddUser();
                }
            }
            
            string jsonString = File.ReadAllText(userPath);
            User user = JsonConvert.DeserializeObject<User>(jsonString);

            this.Username = user.Username;
            this.Email = user.Email;
            this.AccessToken = user.AccessToken;
        }
    }
}