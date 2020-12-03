using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.IO;      //para la imagen 
using Cupcakes.Data;           // para  el contexto
using Cupcakes.Models;         // paro los datos
using Microsoft.EntityFrameworkCore;  // para el EF

namespace Cupcakes.Repositories
{
	public class CupcakeRepository : ICupcakeRepository
	{
		private CupcakeContext _context;     //injeccion de dependia del contexto

		public CupcakeRepository(CupcakeContext context)
		{
			_context = context;
		}

		public IEnumerable<Cupcake> GetCupcakes()
		{
			return _context.Cupcakes.ToList();
		}

		public Cupcake GetCupcakeById(int id)
		{
			return _context.Cupcakes.Include(b => b.Bakery)
				 .SingleOrDefault(c => c.CupcakeId == id);
		}

		public void CreateCupcake(Cupcake cupcake)
		{
			if (cupcake.PhotoAvatar != null && cupcake.PhotoAvatar.Length > 0)
			{
				cupcake.ImageMimeType = cupcake.PhotoAvatar.ContentType;
				cupcake.ImageName = Path.GetFileName(cupcake.PhotoAvatar.FileName);
				using (var memoryStream = new MemoryStream())
				{
					cupcake.PhotoAvatar.CopyTo(memoryStream);
					cupcake.PhotoFile = memoryStream.ToArray();
				}
				_context.Add(cupcake);
				_context.SaveChanges();
			}
		}

		public void DeleteCupcake(int id)
		{
			var cupcake = _context.Cupcakes.SingleOrDefault(c => c.CupcakeId == id);
			_context.Cupcakes.Remove(cupcake);
			_context.SaveChanges();
		}

		public void SaveChanges()
		{
			_context.SaveChanges();
		}

		public IQueryable<Bakery> PopulateBakeriesDropDownList()  // va a llenar un DropDownList con las Bakeries 
		{
			var bakeriesQuery = from b in _context.Bakeries
								orderby b.BakeryName
								select b;
			return bakeriesQuery;
		}
	}
}
