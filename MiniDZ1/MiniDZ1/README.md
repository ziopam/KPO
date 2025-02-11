# �������� �������

## Animals :monkey_face: :rabbit: :tiger:  :wolf: 

### Animal
���������� ����� <span style="color:yellow">Animal</span>, ������� 
����������� �� ����������� <span style="color:yellow">IAlive</span>, 
<span style="color:yellow">IInventory</span>, �, ��������������, ����� ���� 
<span style="color:lime">Food, Health, Number</span>. ����� ������������� ������� ���� <span style="color:lime">SpecieName</span>
� <span style="color:lime">Nickname</span>. ������������ ��������.

+ <span style="color:lime">Food</span> - ���������� ���, ������� �������� ������� �� ���� ����
+ <span style="color:lime">Health</span> - �������� ���������
+ <span style="color:lime">Number</span> - ����� ��������� (��� ��������������). �� ��������� ����� -1.
+ <span style="color:lime">SpecieName</span> - �������� ���� ���������
+ <span style="color:lime">Nickname</span> - ������ ���������

### Herbo
���������� ����� <span style="color:yellow">Herbo</span>, ������� ����������� �� <span style="color:yellow">Animal</span> � �������������
������� ���� <span style="color:lime">Kindness</span> � ����� <span style="color:lime">IsInteractive</span>. ������������ ���������� ��������.

+ <span style="color:lime">Kindness</span> - ������� ���������. �������� �������, ����� ���� �� 0 �� 10, �������
<span style="color:red">ArgumentException</span>, ���� ���������� � ����������� �������� �� ������ � ���� ��������.
+ <span style="color:lime">IsInteractive()</span> - �����, ������� ���������� <span style="color:yellow">true</span>, ���� �������� �����������, ����� <span style="color:yellow">false</span>

����� ������������ ����� <span style="color:lime">ToString()</span>, ������� ���������� ������ �� ���� ����������� � ��������.


### Predator
����������� ����� <span style="color:yellow">Predator</span>, ������� ����������� �� <span style="color:yellow">Animal</span> � �������������
������� ���� <span style="color:lime">Aggressiveness</span>. ������������ ������ ��������.

+ <span style="color:lime">Aggressiveness</span> - ������������� ���������. ����������� ������ �� ����� � ������������ ������ ��� ������ ���������� � ��������
, �� ��������� ���� �� ��������� ���������� �������� 
�� �������� ��������� ���������� (Kindness vs Aggressiveness), ����� ��� ����� ����������� ��� ������ ���������� ������, �������� �������. ����� �� �������, ����� ���� ������ �� 0 �� 10, �������
<span style="color:red">ArgumentException</span>, ���� ���������� � ����������� �������� �� ������ � ���� ��������.

����� ������������ ����� <span style="color:lime">ToString()</span>, ������� ���������� ������ �� ���� ����������� � ��������.

### Monkey, Rabbit, Tiger, Wolf
������, ������� ����������� �� <span style="color:yellow">Herbo</span> � <span style="color:yellow">Predator</span> �������������� �� ����. ����� ������ �������
���������� ���������� <span style="color:lime">SpecieName</span> ��� ������� ���������.
������������ ���������� ���� ��������.

<br>

## HealthCheckers :hospital:

### IAnimalHealthChecker
��������� <span style="color:yellow">IAnimalHealthChecker</span>, ������� �������� ����� <span style="color:lime">CheckHealth</span>(<span style="color:yellow">Animal</span>), ������� ��������� �������� ���������.


### VeterinaryClinic
����� <span style="color:yellow">VeterinaryClinic</span>, ������� ��������� ��������� <span style="color:yellow">IAnimalHealthChecker</span>. �� �����,
��� �� ��������� � ������ ������/����, ��, � ������ ������, ���������� ������ ����� ����������. �� ����� ��� ���������� ������ ������ <span style="color:yellow">Zoo</span> �
���������� SOLID ��������� (� ���������� ����). ���������� ������������ �������.

<br>

## Things :package:
### Thing
����������� ����� <span style="color:yellow">Thing</span>, ������� ����������� �� ���������� <span style="color:yellow">IInventory</span> � ����� ���� <span style="color:lime">Name</span> � <span style="color:lime">Number</span> (�� ��������� -1). 
���������� ����.

### Table, Chair
������, ������� ����������� �� <span style="color:yellow">Thing</span> � ���������� ���������� ����. ������ �� ��� ��������� ���������������  �������� ����
� ���� <span style="color:lime">Name</span>.

<br>


## Zoo :deciduous_tree:
����� <span style="color:yellow">Zoo</span> ������������ ����� �������� �������� ��������, ������� ��������� ������ �������� � �����.
������� ����� ��������������, ��������������� � ������������ �������� � ������������� ���������� � ����� ����������.

**����:**
+ <span style="color:lime">_healthChecker</span> - ������ <span style="color:yellow">IAnimalHealthChecker</span>, ������� ��������� �������� �������� ����� �� ����������� � �������.

+ <span style="color:lime">_animals</span> - ������ ��������, ������������ � ��������.

+ <span style="color:lime">_inventory</span> - ������ �����, ����������� �� ������� ��������.

+ <span style="color:lime">NextNumber</span> - ��������� ��������� ������������������ �����.

**������:**
+ <span style="color:lime">AddAnimal(Animal animal)</span> - ��������� �������� � �������, ���� ��� ������ �������� ��������,
���������� ��� ����������� ����� � ���������� <span style="color:lime">_nextNumber</span>. ������� ��������� ��������� � ���������� ����������.

+ <span style="color:lime">AddThing(Thing thing)</span> - ��������� ����� ������ � ����������� ������ ��������, ���������� ��� ����������� ����� � ���������� <span style="color:lime">_nextNumber</span>. 
������� ��������� ��������� � ���������� ����������.

+ <span style="color:lime">GetTotalFoodConsumption()</span> - ���������� ����� ���������� ���, ����������� ���� �������� �������� � �����.

+ <span style="color:lime">GetAllAnimals()</span> - ���������� ������ ���� �������� � ��������.

+ <span style="color:lime">GetHerbos()</span> - ���������� ������ ���� ���������� ��������, ������������ � ��������.

+ <span style="color:lime">GetInteractiveAnimals()</span> - ���������� ������ ����������� ����������, ������� ����� ��������� � ���������� �������.
���������� ����� <span style="color:lime">IsInteractive()</span> �� ������ <span style="color:yellow">Herbo</span>.

+ <span style="color:lime">GetPredators()</span> - ���������� ������ ���� ��������, ������������ � ��������.

+ <span style="color:lime">GetInventory()</span> - ���������� ������ ���� �����, ����������� �� ����������� �����.


<br>

## Program :computer:
����� ���������� ������������ ������ ���� ������� � �������. ��������� ������� ��������, ��������, �����, ������������ ������� � ������������ ��� ��������� �������� � ����
(��� ���� ������� � ������������ ������� ������� �� ��������� DI ����������).
������� ����������� ���������� ��������, ����� �������, ����� ����. ����� ��������� ���������� � ���� ��������, �����, ����� ���������� ���, ����������� �������� � ��������.
������� ���������� ����� ���� �����, ��������� ������ ������� ������ ���������� � �� ����� ��������������. ����� �������� ����������� �������, ����� ��������� ��������� ���������.

� �������� ���� ����������, ��� ���� ������ ����� ������� ����� �� ���������, ��� ��� ��� �� �������� ������ �������.
���������� ������, � ��� ���������� � �������� � ����� �������� � ����.

<br>

# SOLID

## S � Single Responsibility Principle (������� ������������ ���������������)
������ ����� ��������� ������ **����**(����) ������:

+ <span style="color:yellow">Zoo</span> � ��������� ��������� � ���������� (�������� �� ������ ������ ��������).
+ <span style="color:yellow">VeterinaryClinic</span> � �������� �� �������� �������� ��������.
+ <span style="color:yellow">Animal, Herbo, Predator</span> � ������� ������ ��� ������������� ��������. 
+ <span style="color:yellow">Thing</span> � ������� ����� ��� ��������� ���������.

(���� �������� � ���������� ����������, ����� ���, ��������, <span style="color:yellow">Monkey</span> ��� <span style="color:yellow">Table</span>).

��� ������������ ���������� ������������, ������������ ��������������� �������.


## O � Open/Closed Principle (������� ����������/����������)
������� ����� �����������, �� �� ��������������:

+ ����� �������� ����� ��������, ������ ����������� �� �� <span style="color:yellow">Herbo</span> ��� <span style="color:yellow">Predator</span>.
+ ����� ������� ����� ���� �����, ����������� �� �� <span style="color:yellow">Thing</span>.
+ ����� �������� ����� ������� �������� ��������, ���������� <span style="color:yellow">IAnimalHealthChecker</span>.
+ ��� ��� ����� ������� ��� ��������� ������������� ����.

## L � Liskov Substitution Principle (������� ����������� ������� ������)
��� ��������� ����� �������� ������������ ������ ��� ��������� ������:

+ <span style="color:yellow">Rabbit</span>, <span style="color:yellow">Monkey</span> ��������� �������� ���, ��� ��������� <span style="color:yellow">Herbo</span>.
+ <span style="color:yellow">Tiger</span>, <span style="color:yellow">Wolf</span> ��������� �������� ���, ��� ��������� <span style="color:yellow">Predator</span>.
+ <span style="color:yellow">Table</span>, <span style="color:yellow">Computer</span> ��������� �������� ���, ��� ��������� <span style="color:yellow">Thing</span>.

## I � Interface Segregation Principle (������� ���������� ����������)
���������� ��������� �� ���������� ������������:

+ <span style="color:yellow">IAlive</span> � ��� ����� �������.
+ <span style="color:yellow"> IInventory</span> � ��� ���������.
+ <span style="color:yellow">IAnimalHealthChecker</span> � �������� ������ �� �������� �������� ��������.

## D � Dependency Inversion Principle (������� �������� ������������)
������� �� ������� �� ���������� ���������� �������, � �������� � ����������� <span style="color:yellow">IAnimalHealthChecker</span>.
��� ��������� �������� ����������� ������ ��� ��������� ���� <span style="color:yellow">Zoo</span> (��������, �������� ������ �������� �������� �������� ����� ������������� ������� ���. ����. ).

<br>

# DI ���������
� ������ ��������� ����������� DI ���������, ������� ��������� ��������� <span style="color:yellow">IAnimalHealthChecker</span> � ��� ����������� <span style="color:yellow">VeterinaryClinic</span>.
��� ��������� ����� �������� ���������� ����������, �� ����� ��� ������ <span style="color:yellow">Zoo</span>. <span style="color:yellow">Zoo</span> ��� ��� ��������� ��� ������ <span style="color:lime">GetRequiredService()</span>.

<br>

# ������������
��� ������������ ��� ������ MiniDZ1Test. ����� �������� � �������������� Moq � xUnit.

��������� ������� �������� ���������� ���� �����.

![Tests](tests.png)

��� �������� �������� �������� ������� �������������� ���������� Fine Code Coverage ��� Visual Studio. 

![Test Coverage](test_coverage.png)

����������, ��� �������� ������� ���������� **65.6%**, �� ������� ��������� �� ����� 60%.
������� �� ������ ������ Program.cs, ������� ���������� ������������ ������ ����� ���� ��.

�����, ����� ��������, ��� ��� 37.5% Branch Coverage, �� � ��� �������, ��� �� ������ ����� ������ Line Coverage.
Branch Coverage � � ��������� ����� �� ����, ���� ����� ���������, ��� � ���� ����� ������ � ��������� ��� Exception,
� ��� ����� ��������� � ��������� DI ����������.