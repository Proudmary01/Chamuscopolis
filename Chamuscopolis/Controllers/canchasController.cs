using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using Chamuscopolis.Models;
using Newtonsoft.Json.Linq;

namespace Chamuscopolis.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("")]
    public class canchasController : ApiController
    {
        private prosgEntities db = new prosgEntities();

        // GET: api/canchas
        [Route]
        public List<JObject> Getcanchas(int idComplejo)
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<JObject> _canchas = new List<JObject>();

            IEnumerable<cancha> _cancha = db.canchas.ToList().Where(x => x.SEDE_idSEDE == idComplejo);

            foreach (var cancha in _cancha)
            {
                _canchas.Add(JObject.FromObject(new
                {
                    id = cancha.idCANCHA,
                    tipo = cancha.Ttpo,
                    nombre = cancha.nombre,
                    disponibilidad = cancha.disponibilidad,
                    idComplejo = cancha.SEDE_idSEDE,
                    precio = cancha.precio
                }));
            }

            return _canchas;
        }

        // GET: api/canchas/5
        [ResponseType(typeof(cancha))]
        [Route]
        public async Task<IHttpActionResult> Getcancha(int id_complejo, int id_cancha)
        {
            cancha cancha = await db.canchas.FindAsync(id_complejo, id_cancha);
            if (cancha == null)
            {
                return NotFound();
            }

            JObject _cancha = JObject.FromObject(new
            {
                id = cancha.idCANCHA,
                tipo = cancha.Ttpo,
                nombre = cancha.nombre,
                disponibilidad = cancha.disponibilidad,
                idComplejo = cancha.SEDE_idSEDE,
                precio = cancha.precio
            });

            return Ok(_cancha);
        }

        // PUT: api/canchas/5
        [ResponseType(typeof(void))]
        [Route]
        public async Task<IHttpActionResult> Putcancha(int id, string json)
        {
            JObject o = JObject.Parse(json);
            cancha Cancha = new cancha();

            try
            {
                //Cancha.idCANCHA = (int) o["id"];
                Cancha.idCANCHA = id;
                Cancha.Ttpo = (string) o["tipo"];
                Cancha.nombre = (string) o["nombre"];
                Cancha.disponibilidad = (sbyte) o["disponibilidad"];
                Cancha.SEDE_idSEDE = (int) o["idComplejo"];
                Cancha.precio = (decimal) o["precio"];
            }
            catch (Exception e)
            {
                return BadRequest(ModelState);
            }

            if (id != Cancha.idCANCHA)
            {
                return BadRequest();
            }

            db.Entry(Cancha).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!canchaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/canchas
        [ResponseType(typeof(cancha))]
        [Route]
        public async Task<IHttpActionResult> Postcancha(string json)
        {
            JObject o = JObject.Parse(json);
            cancha Cancha = new cancha();

            try
            {
                //Cancha.idCANCHA = (int)o["id"];
                Cancha.Ttpo = (string)o["tipo"];
                Cancha.nombre = (string)o["nombre"];
                Cancha.disponibilidad = (sbyte)o["disponibilidad"];
                Cancha.SEDE_idSEDE = (int)o["idComplejo"];
                Cancha.precio = (decimal)o["precio"];
            }
            catch (Exception e)
            {
                return BadRequest(ModelState);
            }


            db.canchas.Add(Cancha);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (canchaExists(Cancha.idCANCHA))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = Cancha.idCANCHA }, Cancha);
        }

        // DELETE: api/canchas/5
        [ResponseType(typeof(cancha))]
        [Route]
        public async Task<IHttpActionResult> Deletecancha(int id)
        {
            cancha cancha = await db.canchas.FindAsync(id);
            if (cancha == null)
            {
                return NotFound();
            }

            db.canchas.Remove(cancha);
            await db.SaveChangesAsync();

            return Ok(cancha);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool canchaExists(int id)
        {
            return db.canchas.Count(e => e.idCANCHA == id) > 0;
        }
    }
}