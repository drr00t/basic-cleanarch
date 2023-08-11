using FluentAssertions;
using Stock.Domain;
using Stock.Domain.Entities;

namespace Stock.Testing;

public class ProductTests
{
    [Theory]
    [InlineData("",false)]
    [InlineData(" ",false)]
    [InlineData(null,false)]
    public void Description_is_Invalid(string input, bool result)
    {
        var descr = ProductDescription.From(input);
        descr.ValidationStatus.IsValid.Should().Be(result);
    }
    
    [Theory]
    [InlineData("a description",true)]
    public void Description_is_Valid(string input, bool result)
    {
        var descr = ProductDescription.From(input);
        descr.ValidationStatus.IsValid.Should().Be(result);
    }
    
    [Theory]
    [InlineData("product name", "product description", 34.9, 20.5, 1)]
    public void Product_is_Valid(string inputName,string inputDescription, float inputWeight, float inputPrice, int inputQuantity)
    {
        var name = ProductName.From(inputName);
        var descr = ProductDescription.From(inputDescription);
        var weight = ProductWeight.From(inputWeight);
        var price = ProductPrice.From(inputPrice);
        var quantity = ProductQuantity.From(inputQuantity);
        
        var product = Product.Create(name,descr,weight,price,quantity);
        
        product.IsValid.Should().BeTrue();
        product.GetEvents().Count.Should().Be(1);
    }
}