namespace Ordering.Domain.Tests;

public class DomainAssemblyTests
{
    [Fact]
    public void Domain_assembly_loads()
    {
        Assert.Equal("Ordering.Domain", typeof(Domain.AssemblyReference).Assembly.GetName().Name);
    }
}
