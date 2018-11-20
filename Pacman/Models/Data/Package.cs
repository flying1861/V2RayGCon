using System.Collections.Generic;

namespace Pacman.Models.Data
{
    public class Package
    {
        public List<Bean> beans;
        public string name { get; set; }

        public Package()
        {
            beans = new List<Bean>();
            name = string.Empty;
        }
    }
}
