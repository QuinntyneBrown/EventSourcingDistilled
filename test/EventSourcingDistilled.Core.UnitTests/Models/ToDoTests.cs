using EventSourcingDistilled.Core.Models;
using Xunit;

namespace EventSourcingDistilled.Core.UnitTests.Models
{
    public class ToDoTests
    {
        [Fact]
        public void ShouldCreateValidToDo()
        {
            var name = "Do Shopping";

            var toDo = new ToDo(name);

            Assert.Equal(name, toDo.Name);

            Assert.Single(toDo.DomainEvents);
        }
    }
}
