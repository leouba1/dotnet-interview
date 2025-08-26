using System.ComponentModel.DataAnnotations;
using TodoApi.Dtos.TodoItems;
using TodoApi.Dtos.TodoLists;

namespace TodoApi.Tests.Dtos;

public class DtoValidationTests
{
    [Fact]
    public void CreateTodoItem_MissingDescription_ReturnsRequiredMessage()
    {
        var dto = new CreateTodoItem();
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), results, true);

        Assert.False(isValid);
        var error = Assert.Single(results);
        Assert.Equal("Description is required.", error.ErrorMessage);
    }

    [Fact]
    public void CreateTodoList_MissingName_ReturnsRequiredMessage()
    {
        var dto = new CreateTodoList();
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), results, true);

        Assert.False(isValid);
        var error = Assert.Single(results);
        Assert.Equal("Name is required.", error.ErrorMessage);
    }
}
