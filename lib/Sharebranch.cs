using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace sharedrasil {
    public class SharebranchComparer : IEqualityComparer<Sharebranch> {
        public bool Equals(Sharebranch x, Sharebranch y) {
            return x.Creator.Equals(y.Creator, StringComparison.CurrentCultureIgnoreCase);
        }

        public int GetHashCode(Sharebranch obj) {
            return obj.Creator.GetHashCode();
        }
    }

    public class Sharebranch : ConfigFile {
        public override string PATH {
            get {
                return Path.Combine(Globals.CONFIG_PATH, "sharebranches.json");
            }
        }
        
        public static HashSet<Sharebranch> GetBranchesList() {
            HashSet<Sharebranch> branchList = new HashSet<Sharebranch>(new SharebranchComparer());

            if(File.Exists(Globals.currentBranch.PATH)) {
                string jsonString = File.ReadAllText(Globals.currentBranch.PATH);
                
                JsonConvert.PopulateObject(jsonString, branchList);
            } 

            return branchList; 
        }

        public static void SetCurrent() {
            Globals.currentBranch = Globals.preferences.SignedInBranch;
        }

        public static async Task<Sharebranch> SignIn(string creator) {
            Sharebranch previousBranch = Globals.currentBranch;

            Sharebranch newBranch = new Sharebranch();
            newBranch.CreateRaw(creator);

            Globals.currentBranch = newBranch;

            HashSet<string> shareBuddies = await Github.GetCollaborators();

            if(!(shareBuddies is null)) {
                Github.AddOrigin();

                newBranch.ShareBuddies = shareBuddies;
                newBranch.Save();
                Globals.preferences.SignedInBranch = newBranch;
                Globals.currentUser.Save();
                return newBranch;
            } else {
                Globals.currentBranch = previousBranch;
                return null;
            }
        }

        public string Creator {get; set;}
        public HashSet<string> ShareBuddies {get; set;}
        public string Url {get; set;}
        public string FullUrl {get; set;}

        public void CreateRaw(string creator) {
            this.Creator = creator;
            this.ShareBuddies = new HashSet<string>();
            this.ShareBuddies.Add(creator);
            this.Url = $"{creator}/sharedrasil-{creator}-sharebranch";
            this.FullUrl = $"https://github.com/{this.Url}";
        }
    }
}