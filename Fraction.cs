using System;

class Fraction
{
    private int numerator;
    private int denominator;

    public int Numerator
    {
        get { return numerator; }
        set { numerator = value; }
    }

    public int Denominator
    {
        get { return denominator; }
        set
        {
            if (value != 0)
                denominator = value;
            else
                throw new ArgumentException("Denominator cannot be zero.");
        }
    }

    public Fraction(int numerator, int denominator)
    {
        Numerator = numerator;
        Denominator = denominator;
        Simplify();
    }

    public void Simplify()
    {
        int gcd = GCD(Math.Abs(Numerator), Math.Abs(Denominator));
        Numerator /= gcd;
        Denominator /= gcd;

        if (Denominator < 0)
        {
            Numerator *= -1;
            Denominator *= -1;
        }
    }

    private int GCD(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    public static Fraction operator +(Fraction a, Fraction b)
    {
        int newNumerator = a.Numerator * b.Denominator + b.Numerator * a.Denominator;
        int newDenominator = a.Denominator * b.Denominator;
        return new Fraction(newNumerator, newDenominator);
    }

    public static Fraction operator -(Fraction a, Fraction b)
    {
        int newNumerator = a.Numerator * b.Denominator - b.Numerator * a.Denominator;
        int newDenominator = a.Denominator * b.Denominator;
        return new Fraction(newNumerator, newDenominator);
    }

    public static Fraction operator *(Fraction a, Fraction b)
    {
        int newNumerator = a.Numerator * b.Numerator;
        int newDenominator = a.Denominator * b.Denominator;
        return new Fraction(newNumerator, newDenominator);
    }

    public static Fraction operator /(Fraction a, Fraction b)
    {
        if (b.Numerator == 0)
            throw new DivideByZeroException("Cannot divide by zero.");

        int newNumerator = a.Numerator * b.Denominator;
        int newDenominator = a.Denominator * b.Numerator;
        return new Fraction(newNumerator, newDenominator);
    }

    public override string ToString()
    {
        return $"{Numerator}/{Denominator}";
    }
    public static bool operator <(Fraction a, Fraction b)
    {
        return (a.Numerator * b.Denominator) < (b.Numerator * a.Denominator);
    }

    public static bool operator >(Fraction a, Fraction b)
    {
        return (a.Numerator * b.Denominator) > (b.Numerator * a.Denominator);
    }

    public static bool operator ==(Fraction a, Fraction b)
    {
        return a.Numerator == b.Numerator && a.Denominator == b.Denominator;
    }

    public static bool operator !=(Fraction a, Fraction b)
    {
        return !(a == b);
    }
    public static Fraction operator -(Fraction a)
    {
        return new Fraction(-a.Numerator, a.Denominator);
    }

    public int CompareTo(Fraction other)
    {
        // Implementing the IComparable<T> interface to enable sorting
        if (this < other) return -1;
        if (this > other) return 1;
        return 0;
    }
}