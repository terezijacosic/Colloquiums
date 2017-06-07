using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt
{
    public class Student 
    {
        public String Ime;
        public String Prezime;
        public String Kolegij;

        public Student(String ime, String prezime, String kolegij)
        {
            Ime = ime; Prezime = prezime; Kolegij = kolegij;
        }

        public override string ToString()
        {
            return Ime + " " + Prezime + ", " + Kolegij;
        }
    }
}
