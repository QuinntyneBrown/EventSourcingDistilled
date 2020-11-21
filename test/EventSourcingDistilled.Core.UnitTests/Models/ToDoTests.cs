using EventSourcingDistilled.Core.DomainEvents;
using EventSourcingDistilled.Core.Models;
using Xunit;

namespace EventSourcingDistilled.Core.UnitTests.Models
{
    public class ToDoTests
    {
        [Fact]
        public void Should()
        {
            var toDo = new ToDo();

            Assert.Null(toDo.Name);

            toDo.Apply(new ToDoCreated("Do shopping"));

            Assert.Equal("Do shopping", toDo.Name);

            Assert.Single(toDo.DomainEvents);
        }
    }
}
