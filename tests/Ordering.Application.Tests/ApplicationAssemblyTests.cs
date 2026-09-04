namespace Ordering.Application.Tests;

public class ApplicationAssemblyTests
{
    [Fact]
    public void Application_assembly_loads()
    {
        Assert.Equal("Ordering.Application", typeof(Application.AssemblyReference).Assembly.GetName().Name);
    }
}
