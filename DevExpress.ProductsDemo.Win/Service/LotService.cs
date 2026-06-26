using DevExpress.ProductsDemo.Win.Domain;
using DevExpress.ProductsDemo.Win.Repositories;
using System;
using System.Collections.Generic;

namespace DevExpress.ProductsDemo.Win.Services
{
    public class LotService 
    {
        private readonly ILotRepository _repository;

        public LotService()
        {
            _repository = new LotRepository();
        }

        public List<LotGridModel> GetGridData()
        {
            return _repository.GetGridData();
        }

      
        public Lot GetById(int id)
        {
            return _repository.GetById(id);
        }

        public int Create(Lot lot)
        {
            Validate(lot);

            return _repository.Insert(lot);
        }

        public bool Update(Lot lot)
        {
            Validate(lot);

            return _repository.Update(lot);
        }

        public bool Delete(int id)
        {
            return _repository.Delete(id);
        }

        private static void Validate(Lot lot)
        {
            if (lot == null)
                throw new Exception("الحصة غير موجودة");

            if (string.IsNullOrWhiteSpace(lot.LotName))
                throw new Exception("اسم الحصة إجباري");

            if (lot.LotNumber <= 0)
                throw new Exception("رقم الحصة غير صحيح");

            if (lot.PhysicalProgress < 0 ||
                lot.PhysicalProgress > 100)
                throw new Exception("نسبة الإنجاز يجب أن تكون بين 0 و 100");
             
            if (lot.LotBudget < 0)
                throw new Exception("مبلغ الحصة غير صحيح");

            if (lot.RegisteredAmount < 0)
                throw new Exception("الاعتماد غير صحيح");

            if (lot.ConsumedAmount < 0)
                throw new Exception("الاستهلاك غير صحيح");
        }
    }
}