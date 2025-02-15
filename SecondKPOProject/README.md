# Семинар #2: Принципы SOLID

*Выпускаемые НИУ ВШЭ автомобили с педальными двигателями пользуются большим спросом, но иногда встречаются люди, которые не могут использовать новый тип автомобиля - их ноги слишком слабые,
чтобы крутить педали с достаточной силой. Но студенты нашли решение для этой проблемы, выпустив новый тип двигателя - с ручным приводом. Теперь больше людей могут приобрести автомобили,
выпускаемые НИУ ВШЭ. Студенты обратились к вам для доработки информационной системы под новые реалии.*

![Ручной автомобиль](./Resources/hand-car.jpg)

## Функциональные требования

1. Информационная система должна учитывать автомобили с обоими типами двигателей
2. Автомобили с педальным приводом необходимо продавать покупателям с силой ног больше 5
3. Автомобили с ручным приводом необходимо продавать покупателям с силой рук больше 5

## Требования к реализации

1. Добавить интерфейс `IEngine` для описания двигателя. Двигатель должен сообщать свой тип, а также у покупателя должна иметь возможность понять, подходит ли ему данный тип двигателя
2. Добавить две реализации интефейса `IEngine` - для педального двигателя и для двигателя с ручным приводом. При этом педальный всё также должен иметь размер педалей
3. За счет реализации паттерна "Фабрика" добавить возможность для класса `Car` иметь внутри любой из двух типов двигателя, при этом не теряя композицию с объектом двигателя
4. Добавить класс `CarWarehouse` - он будет принимать машины из различных цехов и поставлять их в автомобильный салон
5. Реализовать принцип сегрегации интерфейсов для класса `CarWarehouse`. Отдельно должны осуществляться операции приема автомобилей из цехов и поставка их в автомобильный салон
6. Добавить классы `PedalCarFactory` и `HandCarFactory`, описывающие соответственно цеха по производству автомобилей с педальным и ручным двигателями
7. В добавленных классах цехов реализовать агрегацию с интерфейсом класса `CarWarehouse`, отвечающим за добавление машин на склад
8. В классах цехов добавить методы для создания автомобилей
9. Добавить класс `CustomerStorage` - он будет вести учет покупателей
10. Реализовать принцип сегрегации интерфейсов для класса `CustomerStorage`. Отдельно должны осуществляться операции добавления покупателей и их получения из хранилища
11. Класс `HseCarFactory` необходимо переименовать в `HseCarShop`.
12. Функционал по хранению автомобилей и покупателей удалить и заменить на агрегацию с частями `CustomerStorage` и `CarWarehouse`, отвечающими за получение данных.
13. Метод `SaleCar` класса `HseCarShop` перед продажей должен проверять, подходит ли автомобиль покупателю