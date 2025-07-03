using System;


public interface ITable
{
    public Type Type { get; }
    public abstract void CreateTable();

}