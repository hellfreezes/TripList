using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripList
{
    public class TripTicket
    {
        private double      gasRateSummer;  //Расход
        private double      gasRateWinter;  //Расход
        private double      gasLiters;      //Количество литров
        private DateTime    receiptDate;    //Дата чека
        private int         homeWay;        //Расстояние до дома
        private int         lastListOdo;    //Показания одометра из предыдущего листа


        //TODO: эти величины должны изменяться продвинутыми настройками из программы
        private int inaccuracy = 10; //Допустимая погрешность в киллометраже +/-
        private bool backToBase = true; //Нужно ли возвращаться на базу каждый раз после посещения очередной точки
        private int averageSpeed = 20; //средняя скорость движения
        private int pause = 15; //время потраченное в точке назначения
        private double fuelLost = 1.0; //Остаток топлива в конце дня которое можно не переносить на следующий день (топливо не обработается вообще)
        private TimeSpan endOfWorkDay = new TimeSpan(17, 0, 0); //конец рабочего дня 18:00
        private TimeSpan startOfWorkDay = new TimeSpan(9, 0, 0); //начало рабочего дня 9:00
        

        public int Id { get; set; }
        public int TotalDistance { get; set; } //Всего пройдено на этом чеке
        public int TotalRealDistance { get; set; }
        public int LastTotalDistance { get; set; } // Общий путь по предыдущим путивым листам

        public double FuelResidude { get; set; }
        public int OdometerStartDay { get; set; }
        public int OdometerEndDay { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }


        private List<Address> usedPOI; //Лист задействованных адресов (чтобы не использовать их повторно)
        private List<Address> pool; //Тут ссылки на все адреса, которые у нас есть, с ним работаем
        private List<POI> poi; //Тут храним все посещенные точки. Для последующего подставления в XLS файл

        private Random random;

        public List<POI> GetPOIs()
        {
            return poi;
        }

        public TripTicket()
        {
            random = new Random();

            Id = 0;

            gasRateSummer = MainWindow.Instance.GetGasRateSummer();
            gasRateWinter = MainWindow.Instance.GetGasRateWinter();
            gasLiters = MainWindow.Instance.GetGasLiters();
            receiptDate = MainWindow.Instance.GetReceiptDate();
            homeWay = MainWindow.Instance.CurrentOptions.Vehicles[MainWindow.Instance.CurrentOptions.SelectedVehicle].HomeWay;
            lastListOdo = 0;

            usedPOI = new List<Address>();
            pool = MainWindow.Instance.GlobalAddressBook.addresses.ToList();
            poi = new List<POI>();

            UpdateOptions();
        }

        public TripTicket(double liters, DateTime date, int id, int lastListOdometer)
        {
            random = new Random();

            lastListOdo = lastListOdometer;
            Id = id;

            homeWay = MainWindow.Instance.CurrentOptions.Vehicles[MainWindow.Instance.CurrentOptions.SelectedVehicle].HomeWay;
            gasRateSummer = MainWindow.Instance.GetGasRateSummer();
            gasRateWinter = MainWindow.Instance.GetGasRateWinter();
            gasLiters = liters;
            receiptDate = date;


            usedPOI = new List<Address>();
            pool = MainWindow.Instance.GlobalAddressBook.addresses.ToList();
            poi = new List<POI>();

            UpdateOptions();
        }

        //Забрать настройки из глобальных настроек программы
        private void UpdateOptions()
        {
            Options o = MainWindow.Instance.CurrentOptions;

            if (MainWindow.Instance.CurrentOptions != null)
            {
                inaccuracy = o.Inaccuracy;
                backToBase = o.BackToBase;
                averageSpeed = o.AverageSpeed;
                pause = o.Pause;
                fuelLost = o.FuelLost;
                endOfWorkDay = TimeSpan.Parse(o.EndOfWorkDay);
                startOfWorkDay = TimeSpan.Parse(o.StartOfWorkDay);
    }
        }

        public NextTripTicket Generate()
        {
            //Вычисление какой расход. Зимний или Летний
            double gasRate = gasRateWinter;
            if ((receiptDate.Month >= 4 && receiptDate.Month <= 9) || (receiptDate.Month == 3 && receiptDate.Month >= 15))
            {
                gasRate = gasRateSummer;
            }

            TotalDistance = GetDistance(gasRate, gasLiters);

            int distance = 0; //счетчик пройденного пути
            int index = 0;

            POI start = new POI() { address = pool[0] }; //TODO: подразумевается, что база имеет индекс 0, а что если это не так!?
            index++;
            start.Id = index;
            poi.Add(start); //<--------------------------------------------------------------------------------------------- Добавляем первую точку. Это база!

            POI prev = start; //предыдущая точка для следующей найденной
            POI curr; //текущая точка. сюда будт получено значение в цикле (раз за разом)

            bool nowBase = true; //изначально мы на базе

            //цикл генерирует первичный маршрут, случайным образом
            //пока пройденный путь меньше общего пройденного пути по расходу/чеку, цикл будет добавлять точки и прибавлять
            //дистанцию до точек в счетчик пройденного пути
            while (distance <= TotalDistance - inaccuracy)
            { //с учетом погрешности
                int min = 0;
                if (nowBase) // Это чтобы находясь на базе случайно не поехать опять на базу
                    min++;
                int r = random.Next(min, pool.Count()); //случайный индекс в диапазоне листа адресов (пула)

                //Если мы не на базе, а нам нужно каждый раз возвращаться на базу, то след адрес у нас БАЗА
                if (!nowBase && backToBase)
                {
                    r = 0; //TODO: тут я подразумеваю, что база имеет у нас индекс 0 в листе адресов
                           //TODO: а что если это не так?
                }

                Address adr = pool[r]; //получаем адрес по индексу
                int poiDistance = adr.Distance;

                //Если мы не на базе, а нам туда нужно каждый раз возвращаться, то дистанция будет равной
                //пройденной перед этим
                if (!nowBase && backToBase)
                {
                    poiDistance = prev.address.Distance;
                    //System.out.println("Предыдущий адрес: " + prev.address.getName());
                }

                //System.out.println("Добавляем: "+poiDistance);

                //проверяем не выйдим ли мы за рамки
                // необходимого пути с учетом допустимой погрешности
                // либо мы не на базе, а нам туда каждый раз надо возвращаться, тогда у нас просто нет выбора
                if ((poiDistance + distance <= TotalDistance + inaccuracy) || (!nowBase && backToBase))
                {
                    distance += poiDistance; //добавляем пройденную дистанцию

                    //Создаем точку для листа с найденным адресом, предыдущей точкой, а следующая точка пока null
                    //Если - это не последняя точка то она будет присвоена в след витке.
                    curr = new POI { address = adr, prev = prev };   //(adr, prev, null);
                    index++;
                    curr.Id = index;
                    poi.Add(curr);

                    if (backToBase)
                    {//Если мы каждый раз возвращаемся на базу, то
                        nowBase = !nowBase; //меняем значение база ли это на противоположное
                    }

                    //Если текущая точка не является первой (есть предыдущая), то
                    //говорим предыдущей точке, что следующая для нее - это текущая
                    int prevDist = 0;

                    if (prev != null)
                    {
                        //В предыдущую точку добавляем дистанцию до текущей точки
                        if (curr.address.IsBase)
                        { //Если теущая точка база, то дистанцию до нее такаяже как и в предыдущую
                            prevDist = prev.address.Distance;
                        }
                        else
                        {
                            prevDist = curr.address.Distance;
                        }

                        //Присвиваем в прыдущую точку дистанцию до текущей
                        prev.distToNext = prevDist;

                        //Записываем остаток топлива
                        gasLiters = gasLiters - (gasRate * prevDist / 100);
                        prev.FuelResidude = gasLiters;
                        //Console.WriteLine("Осталось топилва: "+gasLiters);

                        //Рассчитываем время из предыдущей точки в текущую, на основе дистанции <-----------------------------ВРЕМЯ!!!!!
                        //TimeSpan timeInMinutes = GetTravelTime(averageSpeed, prevDist);
                        prev.toNextPOI = GetTravelTime(averageSpeed, prevDist); //.setMinutesFromMidnight(timeInMinutes); //записываем в предыдущую точку время затраченной для
                                                                                //достижения текущей точки

 
                        //В прыдыдущей точке делаем ссылку на текущую в переменную следующая.
                        prev.next = curr;
                        //System.out.println("Предыдущая: "+prev.address.getName());
                    }

                    //TODO: Где то тут надо сгенерировать дату в точку и время

                    //Для следующего витка цикла в предыдущую точку присваиваем ссылку на текущую
                    prev = curr;

                    usedPOI.Add(adr); //добавляем использованный адрес в соотв лист
                }

                //TODO: не совсем уверен нужно ли так делать: Я удаляю провернную точку из пула даже если она не подошла,
                //чтобы не зациклиться до бесконечности
                if (r != 0)
                { //Если адрес не база, то удаляем
                    pool.Remove(pool[r]);
                }

                //После удаления я проверяю есть ли в пуле еще точки и если их нет, то цикл я прерываю,
                //т.к. не хватает точек достроить маршрутный лист
                if (pool.Count <= 1)
                { //1 - потому, что в конечном итоге база то останется
                    //System.out.println("Перебрали все значение, но подходящего нет! Маршрутный лист недостроен.");
                     break;
                }
            } //КОНЕЦ ЦИКЛА

            TotalRealDistance = distance;

            // По уже созданному маршруту генерируются даты-время пути.
            // Если требуется создавать один маршрутный на один день,
            // И если в один день уложиться не удалось, то
            // ntt веренется не null
            // А это значит что по данным ntt будет сгенерирован следующий лист на след день
            // Поэтому:
            NextTripTicket ntt = GenerateTimeAndDate(poi);

            // В пробег начального дня прибавляется путь до дома
            // ПЛЮС путь ОТ дома (чтоб туда обратно получилось)
            if (Id != 0)
                OdometerStartDay = lastListOdo + homeWay * 2;

            if (ntt != null)
            {
                TotalRealDistance = 0; // Обнуляем пройденный путь в рамках одного листа
                poi.RemoveRange(ntt.NumberPOI, poi.Count-ntt.NumberPOI); // Удаляем точки которые не будут использоваться данным листом

                // Пробегаемся по всем точкам чтобы...
                foreach (POI p in poi)
                {
                    // ... суммировать весь пройденный путь только по этим точкам
                    TotalRealDistance += p.distToNext;
                }

                ntt.OdometerEndDay = OdometerStartDay + TotalRealDistance;

                //return ntt; <Возвращаем ниже>// вернем данные для генерации след листа
                              // таким образом, код который вызвал генерацию будет знать что
                              // генерация была ограничена одинм днем и весь бензин еще не использован
                              // и генерация должна быть продолжена с использованием выходных данных ntt
            }

            OdometerEndDay = OdometerStartDay + TotalRealDistance;

            TimeSpan randTime = new TimeSpan(0, random.Next(2, 15), 0);
            // Время Выезда по путевому
            StartTime = poi[0].timeDeparture - randTime;

            randTime = new TimeSpan(0, random.Next(2, 15), 0);
            int k = poi.Count - 1;

            // TODO: Это костыль:
            //       Иногда последняя точка путая и время у нее равно нулю. Поэтому вернем индекс предпоследней
            if (poi[k].timeArrive == new DateTime(1, 1, 1, 0, 0, 0))
                k--;
            // Время приезда по путевому
            EndTime = poi[k].timeArrive + randTime;

            return ntt; // если ntt вернулось null то генерация след листа не требуется.
        }

        private NextTripTicket GenerateTimeAndDate(List<POI> poi)
        {
            int i = 1;

            NextTripTicket ntt = null;

            BusinessDaysCalculator businessDays = MainWindow.Instance.GlobalBusinessDaysCalculator;

            // Стартовая дата в чеке
            DateTime startTime = receiptDate;

            //Определяем конец рабочего дня (попросту добавляем к указанной дате время)
            DateTime endDT = new DateTime(startTime.Year, startTime.Month, startTime.Day, endOfWorkDay.Hours, endOfWorkDay.Minutes, 0);
            
            foreach (POI p in poi)
            {
                if (i > 1)
                {
                    startTime = startTime.AddMinutes(p.prev.toNextPOI.TotalMinutes);
                }
                if (p.next == null)
                    break;

                //ПРЕДСТАВЛЕНИЕ ДАТЫ
                //Конвертация даты в строку и запись в объект. (Можно делать это уже при выгрузки в XLSX
                p.Date = startTime.ToShortDateString();
                p.FullDate = startTime;

                startTime = startTime.AddMinutes(random.Next(0, pause+1)); //Добавляем задержку в точке назначения

                p.timeDeparture = startTime;
                p.timeArrive = startTime.Add(p.toNextPOI);

                //Проверяем укладываемся ли в рабочий день
                if (p.next != null)
                {
                    //Если ехать в следующую точку, то надо проверить а хватит ли потом времени в рамках рабочего дня

                    if ((startTime.AddMinutes(p.next.toNextPOI.TotalMinutes * 2) > endDT) && !p.address.IsBase)
                    {
                        DateTime checkDay;
                        //Если не хватит то переключаем день на следующий
                        do
                        { //Меняем день до тех пор пока он не станет следующим РАБОЧИМ днем
                            startTime = startTime.AddDays(1);
                            checkDay = new DateTime(startTime.Year, startTime.Month, startTime.Day);

                        } while (((startTime.DayOfWeek == DayOfWeek.Saturday || startTime.DayOfWeek == DayOfWeek.Sunday) && (businessDays.IsBusinessDay(checkDay) != 1)) || // если это выходной и не является принудительно рабочим
                                    businessDays.IsBusinessDay(checkDay) == 2); // если это любой другой день, но он объявлен нерабочим

                        //Переводим стартовое время на начало рабочего дня
                        startTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, startOfWorkDay.Hours, startOfWorkDay.Minutes, 0);
                        //Определяем конец рабочего дня
                        endDT = new DateTime(startTime.Year, startTime.Month, startTime.Day, endOfWorkDay.Hours, endOfWorkDay.Minutes, 0);
                        //startTime = startOfWorkDay;

                        FuelResidude = Math.Round(p.FuelResidude);
                        // Если нужно создавать новый трип лист каждый раз когда закончился день
                        // и еще достаточно топлива, то
                        if (MainWindow.DIVIDE_TRIPTICKETS && FuelResidude > fuelLost)
                        {
                            // Записываем данные для переноса на след день
                            ntt = new NextTripTicket()
                            {
                                NextIndex = Id + 1, // Id следующего путевого листа
                                Date = startTime, //Дата с которой начнется след лист
                                Liters = p.FuelResidude, //Остаток топлива с которым начнется день
                                NumberPOI = i //Количество уже пройденных точек (нужно для удаление лишних)
                            };

                            return ntt;
                        }

                        i = 0;
                    }
                    i++;
                }
            }

            return null;
        }

        /**
         * Получить количество пройденных километров
         * @param consumption - текущий расход в этом сезоне
         * @param liters - количество израсходованных литров топлива
         * @return - количество пройденных километров
         */
        private int GetDistance(double consumption, double liters)
        {
            double result = (liters * 100) / consumption;
            return (int)result;
        }

        /**
         * Возвращает количество затраченных на пройденный путь минут
         * @param aSpeed - средняя скорость движения
         * @param dist - пройденный путь в киллометрах
         * @return - возвращает количество затраченных МИНУТ
         */
        private TimeSpan GetTravelTime(int aSpeed, int dist)
        {
            DateTime dt1 = new DateTime(1, 1, 1, 0, 0, 0);
            int result = (dist * 60) / aSpeed; //60 - это минут
            DateTime dt2 = dt1.AddMinutes(result);

            return dt2 - dt1;
        }
    }
}
