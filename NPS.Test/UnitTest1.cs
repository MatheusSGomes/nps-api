namespace NPS.Test;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var expected = 1;
        var actual = 2;

        Assert.NotEqual(expected, actual);
    }
}
