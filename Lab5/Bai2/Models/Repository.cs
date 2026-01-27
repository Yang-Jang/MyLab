using System.Collections.Generic;

namespace Bai2.Models
{
    public class Repository : IRepository
    {
        private Dictionary<int, Reservation> items;

        public Repository()
        {
            items = new Dictionary<int, Reservation>();
            // Tạo dữ liệu mẫu 
            new List<Reservation> {
                new Reservation { Id=1, Name = "Thepv", StartLocation = "Ha Noi", EndLocation="Can Tho" },
                new Reservation { Id=2, Name = "Fpoly", StartLocation = "Sai Gon", EndLocation="Tay Nguyen" }
            }.ForEach(r => AddReservation(r));
        }

        public Reservation this[int id] => items.ContainsKey(id) ? items[id] : null;

        public IEnumerable<Reservation> Reservations => items.Values;

        public Reservation AddReservation(Reservation reservation)
        {
            if (reservation.Id == 0)
            {
                int key = items.Count + 1; // Logic ID tự tăng đơn giản
                while (items.ContainsKey(key)) { key++; };
                reservation.Id = key;
            }
            items[reservation.Id] = reservation;
            return reservation;
        }

        public void DeleteReservation(int id) => items.Remove(id);

        public Reservation UpdateReservation(Reservation reservation) => AddReservation(reservation);
    }
}