/*
 * 27.09.2016 Minor edit
 * 11.11.2009 New package structure
 * 27.10.2006 Original version
 */

namespace Prime;

public class Address
{
    public int Level { get; set; }
    public int Displacement { get; set; }

    public Address()
    {
        Level = 0;
        Displacement = 0;
    }

    public Address(int level, int displacement)
    {
        Level = level;
        Displacement = displacement;
    }

    public Address(Address a, int increment)
    {
        Level = a.Level;
        Displacement = a.Displacement + increment;
    }

    public Address(Address a)
    {
        Level = a.Level + 1;
        Displacement = 0;
    }

    public override string ToString()
    {
        return $"level={Level} displacement={Displacement}";
    }
}