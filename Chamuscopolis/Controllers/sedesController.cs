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
    public class sedesController : ApiController
    {
        private prosgEntities db = new prosgEntities();

        // GET: api/sedes
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
        public async Task<IHttpActionResult> Putsede(int id, string json)
        {
            JObject o = JObject.Parse(json);
            sede Sede = new sede();
            try
            {
                //Sede.idSEDE = (int)o["idComplejo"];
                Sede.idSEDE = id;
                Sede.Nombre = (string) o["nombre"];
                Sede.ciudad = (string)o["direccion"]["ciudad"];
                Sede.calle = (string) o["direccion"]["calle"];
                Sede.avenida = (string) o["direccion"]["avenida"];
                Sede.lat = (decimal) o["direccion"]["geo"]["lat"];
                Sede.lng = (decimal) o["direccion"]["geo"]["lng"];
                Sede.email = (string) o["email"];
            }
            catch (Exception e)
            {
                return BadRequest(ModelState);
            }
            
            if (id != Sede.idSEDE)
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
                if (!sedeExists(id))
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
        public async Task<IHttpActionResult> Postsede(string json)
        {
            JObject o = JObject.Parse(json);
            sede Sede = new sede();
            try
            {
                //Sede.idSEDE = (int)o["idComplejo"];
                Sede.Nombre = (string)o["nombre"];
                Sede.ciudad = (string)o["direccion"]["ciudad"];
                Sede.calle = (string)o["direccion"]["calle"];
                Sede.avenida = (string)o["direccion"]["avenida"];
                Sede.lat = (decimal)o["direccion"]["geo"]["lat"];
                Sede.lng = (decimal)o["direccion"]["geo"]["lng"];
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
        public async Task<IHttpActionResult> Deletesede(string json)
        {
            JObject o = JObject.Parse(json);
            int id = (int) o["idComplejo"];
            sede sede = await db.sedes.FindAsync(id);
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