using RestAppUdemy.Model;
using System.Security.Cryptography;

namespace RestAppUdemy.Service.Implementations
{
    public class PersonServiceImplementation : IPersonService
    {
        private volatile int count;

        public Person Create(Person person)
        {
            return person;
        }

        public void Delete(long id)
        {
        }

        public List<Person> FindAll()
        {
            List<Person> persons = new List<Person>();
            for (int i = 0; i < 10; i++)
            {
                Person person = MockPerson(i);
                persons.Add(person);
            } 
            return persons;
        }

        public Person FindByID(long id)
        {
            return new Person
            {
                Id = IncrementaAndGet(),
                FirstName = "Leandro",
                LastName = "Costa",
                Address = "Uberlândia - Minas Gerais - Brasil",
                Gender = "Male"
            };
         }

        public Person Update(Person person)
        {
            return person;
        }

        private Person MockPerson(int i)
        {
            return new Person
            {
                Id = IncrementaAndGet(),
                FirstName = "Leandro" + i,
                LastName = "Costa" + i,
                Address = "Uberlândia - Minas Gerais - Brasil" + i,
                Gender = "Male" + i
            };
        }

        private long IncrementaAndGet()
        {
            return Interlocked.Increment(ref count);
        }
    }
}
