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
    [RoutePrefix("")]
    public class sedesController : ApiController
    {
        private prosgEntities db = new prosgEntities();

        // GET: api/sedes
        [Route]
        public List<JObject> Getsedes()
        {
            db.Configuration.ProxyCreationEnabled = false;
            List<JObject> _sedes = new List<JObject>();
            foreach (var sede in db.sedes)
            {
                _sedes.Add(JObject.FromObject(new
                {
                    idComplejo = sede.idSEDE,
                    nombre = sede.Nombre,
                    direccion = new
                    {
                        ciudad=sede.ciudad,
                        calle=sede.calle,
                        avenida=sede.avenida,
                        geo = new
                        {
                            lat = sede.lat,
                            lng = sede.lng
                        }
                    },
                    email = sede.email
                }));
            }

            return _sedes;
        }

        // GET: api/sedes/5
        [ResponseType(typeof(sede))]
        [Route]
        public async Task<IHttpActionResult> Getsede(int id)
        {
            sede sede = await db.sedes.FindAsync(id);
            if (sede == null)
            {
                return NotFound();
            }

            JObject _sede = JObject.FromObject(new
            {
                idComplejo = sede.idSEDE,
                nombre = sede.Nombre,
                direccion = new
                {
                    ciudad = sede.ciudad,
                    calle = sede.calle,
                    avenida = sede.avenida,
                    geo = new
                    {
                        lat = sede.lat,
                        lng = sede.lng
                    }
                },
                email = sede.email
            });

            return Ok(_sede);
        }

        // PUT: api/sedes/5
        [ResponseType(typeof(void))]
        [Route]
        public async Task<IHttpActionResult> Putsede(string body) 
        {
            JObject o = JObject.Parse(body);
            int idComplejo = (int)o["idComplejo"];
            sede Sede = new sede();
            try
            {
                Sede.idSEDE = (int)o["idComplejo"];
                Sede.Nombre = (string)o["nombre"];
                Sede.ciudad = (string)o["ciudad"];
                Sede.calle = (string)o["calle"];
                Sede.avenida = (string)o["avenida"];
                Sede.lat = (decimal)o["latitud"];
                Sede.lng = (decimal)o["longitud"];
                Sede.email = (string)o["email"];
            }
            catch (Exception e)
            {
                return BadRequest(ModelState);
            }
            
            if (idComplejo != Sede.idSEDE)
            {
                return BadRequest();
            }

            db.Entry(Sede).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!sedeExists(idComplejo))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.Accepted);
        }

        // POST: api/sedes
        [ResponseType(typeof(sede))]
        [Route]
        public async Task<IHttpActionResult> Postsede(string body)
        {
            JObject o = JObject.Parse(body);
            sede Sede = new sede();
            try
            {
                //Sede.idSEDE = (int)o["idComplejo"];
                Sede.Nombre = (string)o["nombre"];
                Sede.ciudad = (string)o["ciudad"];
                Sede.calle = (string)o["calle"];
                Sede.avenida = (string)o["avenida"];
                Sede.lat = (decimal)o["latitud"];
                Sede.lng = (decimal)o["longitud"];
                Sede.email = (string)o["email"];
            }
            catch (Exception e)
            {
                return BadRequest(ModelState);
            }
            
            db.sedes.Add(Sede);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (sedeExists(Sede.idSEDE))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = Sede.idSEDE }, Sede);
        }

        // DELETE: api/sedes/5
        [ResponseType(typeof(sede))]
        [Route]
        public async Task<IHttpActionResult> Deletesede(int idComplejo)
        {
            //JObject o = JObject.Parse(body);
            //int id = (int) o["idComplejo"];
            sede sede = db.sedes.FirstOrDefault(x => x.idSEDE == idComplejo);
            if (sede == null)
            {
                return NotFound();
            }

            db.sedes.Remove(sede);
            await db.SaveChangesAsync();

            return Ok(sede);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool sedeExists(int id)
        {
            return db.sedes.Count(e => e.idSEDE == id) > 0;
        }
    }
}