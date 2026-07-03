namespace PdfOperations.Tests;

[TestClass]
public class DivideTests
{
    [TestMethod]
    public void OneToMany(string input, string output)
    {
        Divide.OneToMany();
    }
    
    [TestMethod]
    public void ManyToOne(string [] input, string output)
    {
        Divide.ManyToOne();
    }
}