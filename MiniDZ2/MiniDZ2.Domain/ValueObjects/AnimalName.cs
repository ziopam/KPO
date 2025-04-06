﻿namespace MiniDZ2.Domain.ValueObjects
{
    internal record AnimalName
    {
        public string Value { get; }

        public AnimalName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Имя животного не может быть пустым");
            }
            Value = name;
        }
    }
}
