# ❗Файл решения (.sln) находится в папке MiniDZ1❗

# Описание решения

## Animals :monkey_face: :rabbit: :tiger:  :wolf: 

### Animal
Абстракный класс <span style="color:yellow">Animal</span>, который 
наследуется от интерфейсов <span style="color:yellow">IAlive</span>, 
<span style="color:yellow">IInventory</span>, и, соответственно, имеет поля 
<span style="color:lime">Food, Health, Number</span>. Также дополнительно имеются поля <span style="color:lime">SpecieName</span>
и <span style="color:lime">Nickname</span>. Олицитворяет животное.

+ <span style="color:lime">Food</span> - количество еды, которое животное съедает за один день
+ <span style="color:lime">Health</span> - здоровье животного
+ <span style="color:lime">Number</span> - номер животного (для инвентаризации). По умолчанию равен -1.
+ <span style="color:lime">SpecieName</span> - название вида животного
+ <span style="color:lime">Nickname</span> - кличка животного

### Herbo
Абстракный класс <span style="color:yellow">Herbo</span>, который наследуется от <span style="color:yellow">Animal</span> и дополнительно
имеется поле <span style="color:lime">Kindness</span> и метод <span style="color:lime">IsInteractive</span>. Олицитворяет травоядное животное.

+ <span style="color:lime">Kindness</span> - доброта животного. Согласно заданию, может быть от 0 до 10, бросает
<span style="color:red">ArgumentException</span>, если переданное в конструктор значение не входит в этот диапозон.
+ <span style="color:lime">IsInteractive()</span> - метод, который возвращает <span style="color:yellow">true</span>, если животное дружелюбное, иначе <span style="color:yellow">false</span>

Также преопределен метод <span style="color:lime">ToString()</span>, который возвращает строку со всей информацией о животном.


### Predator
Абстрактный класс <span style="color:yellow">Predator</span>, который наследуется от <span style="color:yellow">Animal</span> и дополнительно
имеется поле <span style="color:lime">Aggressiveness</span>. Олицитворяет хищное животное.

+ <span style="color:lime">Aggressiveness</span> - агрессивность животного. Фактической пользы не несет и используется только для вывода информации о хищниках
, но позволяет хотя бы разделить травоядных животных 
от хищников различием параметров (Kindness vs Aggressiveness), чтобы был смысл реализовать два разных абстракных класса, согласно заданию. Также по заданию, может быть только от 0 до 10, бросает
<span style="color:red">ArgumentException</span>, если переданное в конструктор значение не входит в этот диапозон.

Также преопределен метод <span style="color:lime">ToString()</span>, который возвращает строку со всей информацией о животном.

### Monkey, Rabbit, Tiger, Wolf
Классы, которые наследуются от <span style="color:yellow">Herbo</span> и <span style="color:yellow">Predator</span> соответственно их виду. Также классы заранее
определены параметром <span style="color:lime">SpecieName</span> для каждого животного.
Олицитворяют конкретные виды животных.

<br>

## HealthCheckers :hospital:

### IAnimalHealthChecker
Интерфейс <span style="color:yellow">IAnimalHealthChecker</span>, который содержит метод <span style="color:lime">CheckHealth</span>(<span style="color:yellow">Animal</span>), который проверяет здоровье животного.


### VeterinaryClinic
Класс <span style="color:yellow">VeterinaryClinic</span>, который реализует интерфейс <span style="color:yellow">IAnimalHealthChecker</span>. По факту,
мог бы содержать и другие методы/поля, но, в данном случае, реализован только метод интерфейса. Он нужен для корректной работы класса <span style="color:yellow">Zoo</span> и
соблюдения SOLID принципов (о соблюдении ниже). Абстракция ветеринарной клиники.

<br>

## Things :package:
### Thing
Абстрактный класс <span style="color:yellow">Thing</span>, который наследуется от интерфейса <span style="color:yellow">IInventory</span> и имеет поля <span style="color:lime">Name</span> и <span style="color:lime">Number</span> (по умолчанию -1). 
Абстракция вещи.

### Table, Chair
Классы, которые наследуются от <span style="color:yellow">Thing</span> и определяют конкретные вещи. Каждой из них назначено соответствующее  название вещи
в поле <span style="color:lime">Name</span>.

<br>


## Zoo :deciduous_tree:
Класс <span style="color:yellow">Zoo</span> представляет собой основную сущность зоопарка, которая управляет учетом животных и вещей.
Зоопарк ведет инвентаризацию, взаимодействует с ветеринарной клиникой и предоставляет информацию о своих обитателях.

**Поля:**
+ <span style="color:lime">_healthChecker</span> - объект <span style="color:yellow">IAnimalHealthChecker</span>, который проверяет здоровье животных перед их добавлением в зоопарк.

+ <span style="color:lime">_animals</span> - список животных, содержащихся в зоопарке.

+ <span style="color:lime">_inventory</span> - список вещей, находящихся на балансе зоопарка.

+ <span style="color:lime">NextNumber</span> - следующий доступный инвентаризационный номер.

**Методы:**
+ <span style="color:lime">AddAnimal(Animal animal)</span> - добавляет животное в зоопарк, если оно прошло проверку здоровья,
присваивая ему инвентарный номер и увелечивая <span style="color:lime">_nextNumber</span>. Выводит текстовое сообщение о результате добавления.

+ <span style="color:lime">AddThing(Thing thing)</span> - добавляет новый объект в инвентарный список зоопарка, присваивая ему инвентарный номер и увелечивая <span style="color:lime">_nextNumber</span>. 
Выводит текстовое сообщение о результате добавления.

+ <span style="color:lime">GetTotalFoodConsumption()</span> - возвращает общее количество еды, необходимое всем животным зоопарка в сутки.

+ <span style="color:lime">GetAllAnimals()</span> - возвращает список всех животных в зоопарке.

+ <span style="color:lime">GetHerbos()</span> - возвращает список всех травоядных животных, содержащихся в зоопарке.

+ <span style="color:lime">GetInteractiveAnimals()</span> - возвращает список дружелюбных травоядных, которых можно поместить в контактный зоопарк.
Использует метод <span style="color:lime">IsInteractive()</span> из класса <span style="color:yellow">Herbo</span>.

+ <span style="color:lime">GetPredators()</span> - возвращает список всех хищников, содержащихся в зоопарке.

+ <span style="color:lime">GetInventory()</span> - возвращает список всех вещей, находящихся на инвентарном учете.


<br>

## Program :computer:
Здесь происходит демонстрация работы всех классов и методов. Создаются объекты зоопарка, животных, вещей, ветеринарной клиники и производятся все возможные действия с ними
(при этом зоопарк и ветеринарная клиника связана по средствам DI контейнера).
Сначала добавляются травоядные животные, затем хищники, затем вещи. Далее выводится информация о всех животных, вещах, общем количестве еды, дружелюбных животных и хищниках.
Порядок добавления может быть любым, изменится только порядок вывода информации и их номер инвентаризации. Можно поменять добавляемые объекты, чтобы отследить поведение программы.

С лектором было обговорено, что ввод данных через консоль можно не добавлять, так как это не является частью задания.
Достаточно вывода, а вся информация о животных и вещах задается в коде.

<br>

# SOLID

## S – Single Responsibility Principle (Принцип единственной ответственности)
Каждый класс выполняет только **одну**(свою) задачу:

+ <span style="color:yellow">Zoo</span> – управляет животными и инвентарем (отвечает за бизнес логику зоопарка).
+ <span style="color:yellow">VeterinaryClinic</span> – отвечает за проверку здоровья животных.
+ <span style="color:yellow">Animal, Herbo, Predator</span> – базовые классы для моделирования животных. 
+ <span style="color:yellow">Thing</span> – базовый класс для предметов инвентаря.

(тоже касается и конкретных реализаций, таких как, например, <span style="color:yellow">Monkey</span> или <span style="color:yellow">Table</span>).

Это обеспечивает разделение обязанностей, предотвращая перегруженность классов.


## O – Open/Closed Principle (Принцип открытости/закрытости)
Система легко расширяется, но не модифицируется:

+ Можно добавить новых животных, просто унаследовав их от <span style="color:yellow">Herbo</span> или <span style="color:yellow">Predator</span>.
+ Можно создать новые виды вещей, унаследовав их от <span style="color:yellow">Thing</span>.
+ Можно внедрить новую систему проверки здоровья, реализовав <span style="color:yellow">IAnimalHealthChecker</span>.
+ Все это можно сделать без изменения существующего кода.

## L – Liskov Substitution Principle (Принцип подстановки Барбары Лисков)
Все подклассы могут заменять родительские классы без нарушения логики:

+ <span style="color:yellow">Rabbit</span>, <span style="color:yellow">Monkey</span> корректно работают там, где ожидается <span style="color:yellow">Herbo</span>.
+ <span style="color:yellow">Tiger</span>, <span style="color:yellow">Wolf</span> корректно работают там, где ожидается <span style="color:yellow">Predator</span>.
+ <span style="color:yellow">Table</span>, <span style="color:yellow">Computer</span> корректно работают там, где ожидается <span style="color:yellow">Thing</span>.

## I – Interface Segregation Principle (Принцип разделения интерфейса)
Интерфейсы разделены по конкретным обязанностям:

+ <span style="color:yellow">IAlive</span> – для живых существ.
+ <span style="color:yellow"> IInventory</span> – для инвентаря.
+ <span style="color:yellow">IAnimalHealthChecker</span> – отвечает только за проверку здоровья животных.

## D – Dependency Inversion Principle (Принцип инверсии зависимостей)
Зоопарк не зависит от конкретной реализации клиники, а работает с интерфейсом <span style="color:yellow">IAnimalHealthChecker</span>.
Это позволяет заменять проверяющий сервис без изменения кода <span style="color:yellow">Zoo</span> (например, возможно услуги проверки здоровья животных будет предоставлять частный вет. врач. ).

<br>

# DI контейнер
В данной программе использован DI контейнер, который связывает интерфейс <span style="color:yellow">IAnimalHealthChecker</span> с его реализацией <span style="color:yellow">VeterinaryClinic</span>.
Это позволяет легко заменить реализацию интерфейса, не меняя код класса <span style="color:yellow">Zoo</span>. <span style="color:yellow">Zoo</span> при это создается при вызове <span style="color:lime">GetRequiredService()</span>.

<br>

# Тестирование
Для тестирования был создан MiniDZ1Test. Тесты написаны с использованием Moq и xUnit.

Программа успешно проходит написанные мной тесты.

![Tests](tests.png)

Для подсчета процента покрытия тестами использовалось расширение Fine Code Coverage для Visual Studio. 

![Test Coverage](test_coverage.png)

Получается, что покрытие тестами составляет **65.6%**, по заданию требуется не менее 60%.
Тестами не покрыт только Program.cs, который производит демонстрацию работы моего мини дз.

Также, стоит отметить, что тут 37.5% Branch Coverage, но я так понимаю, что по заднию нужно именно Line Coverage.
Branch Coverage я и поправить особо не могу, ведь здесь ожидается, что я буду также ловить и проверять все Exception,
в том числе связанные с созданием DI контейнера.
