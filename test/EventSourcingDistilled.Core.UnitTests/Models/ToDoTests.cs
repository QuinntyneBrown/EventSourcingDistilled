using EventSourcingDistilled.Core.DomainEvents;
using EventSourcingDistilled.Core.Models;
using System.Threading.Tasks;
using Xunit;

namespace EventSourcingDistilled.Core.UnitTests.Models
{
    public class ToDoTests
    {

        [Fact]
        public async Task Should()
        {
            var toDo = new ToDo();

            toDo.Apply(new ToDoCreated("Do shopping"));

            Assert.Equal("Do shopping", toDo.Name);

            Assert.Single(toDo.DomainEvents);
        }
    }
}
