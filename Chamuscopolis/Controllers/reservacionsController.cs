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
    public class reservacionsController : ApiController
    {
        private prosgEntities db = new prosgEntities();

        // GET: api/reservacions
        public IQueryable<reservacion> Getreservacions()
        {
            return db.reservacions;
        }

        // GET: api/reservacions/cancha/
        [ResponseType(typeof(reservacion))]
        public async Task<IHttpActionResult> Getreservacion(int id)
        {
            reservacion reservacion = await db.reservacions.FindAsync(id);
            if (reservacion == null)
            {
                return NotFound();
            }

            return Ok(reservacion);
        }

        // GET: api/reservacions/5
        [ResponseType(typeof(reservacion))]
        public IHttpActionResult GetHorariosReservados(int idCancha, string fecha)
        {
            List<string> horas = new List<string>();
            List<reservacion> reservaciones = db.reservacions.Where(reser => (reser.CANCHA_idCANCHA == idCancha && reser.fecha.ToString() == fecha)).ToList();
            if (reservaciones.Count == 0)
            {
                return Ok(horas);
            }

            foreach (reservacion r in reservaciones)
            {
                horas.Add(r.hora.ToString());
            }
            return Ok(horas);
        }

        // PUT: api/reservacions/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putreservacion(int id, reservacion reservacion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != reservacion.idRESERVACION)
            {
                return BadRequest();
            }

            db.Entry(reservacion).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!reservacionExists(id))
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

        // POST: api/reservacions
        [ResponseType(typeof(reservacion))]
        public async Task<IHttpActionResult> Postreservacion(string json)
        {
            JObject o = JObject.Parse(json);
            reservacion reservacion = new reservacion();
            try
            {
                reservacion.nombreTarjeta = (string)o["nombreTarjeta"];
                reservacion.numeroTarjeta = (string)o["numeroTarjeta"];
                reservacion.cvv = (int)o["codigoSeguridad"];
                reservacion.fechaExp = (DateTime)o["fechaExpiracion"];
                reservacion.tipotarjeta = (int)o["tipoTarjeta"];
                reservacion.monto = (decimal)o["monto"];
                reservacion.CANCHA_idCANCHA = (int)o["idCancha"];
                reservacion.hora = (string)o["horario"];
            }
            catch (Exception e)
            {
                return BadRequest(ModelState);
            }

            db.reservacions.Add(reservacion);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (reservacionExists(reservacion.idRESERVACION))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = reservacion.idRESERVACION }, reservacion);
        }

        // DELETE: api/reservacions/5
        [ResponseType(typeof(reservacion))]
        public async Task<IHttpActionResult> Deletereservacion(int id)
        {
            reservacion reservacion = await db.reservacions.FindAsync(id);
            if (reservacion == null)
            {
                return NotFound();
            }

            db.reservacions.Remove(reservacion);
            await db.SaveChangesAsync();

            return Ok(reservacion);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool reservacionExists(int id)
        {
            return db.reservacions.Count(e => e.idRESERVACION == id) > 0;
        }
    }
}