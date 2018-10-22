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
    public class usuariosController : ApiController
    {
        private prosgEntities db = new prosgEntities();

        // GET: api/usuarios
        [Route]
        public List<JObject> Getusuarios()
        {
            List<JObject> _usuarios = new List<JObject>();

            foreach (var usuario in db.usuarios)
            {
                _usuarios.Add(JObject.FromObject(new
                {
                    dpi = usuario.DPI,
                    nombre = usuario.Nombre,
                    apellido = usuario.Apellido,
                    telefono = usuario.telefono,
                    direccion = usuario.direccion,
                    fechaNacimiento = usuario.fecha_nac,
                    email = usuario.correo,
                    user = usuario.username,
                    password = usuario.password,
                    tipo = usuario.tipo,
                    estado = usuario.estado
                }));

            }

            return _usuarios;
        }

        // GET: api/usuarios/5
        [ResponseType(typeof(usuario))]
        [Route]
        public async Task<IHttpActionResult> Getusuario(string email) //int id
        {
            //JObject o = JObject.Parse(json);
            //string email = (string) o["correo"];

            usuario usuario = db.usuarios.FirstOrDefault(x => (x.correo == email));
            if (usuario == null)
            {
                return NotFound();
            }

            JObject _usuario = JObject.FromObject(new
            {
                id = usuario.idUSUARIO,
                dpi = usuario.DPI,
                nombre = usuario.Nombre,
                apellido = usuario.Apellido,
                telefono = usuario.telefono,
                direccion = usuario.direccion,
                fechaNacimiento = usuario.fecha_nac,
                email = usuario.correo,
                user = usuario.username,
                password = usuario.password,
                tipo = usuario.tipo,
                estado = usuario.estado
            });



            return Ok(_usuario);
        }

        // PUT: api/usuarios/5
        [ResponseType(typeof(void))]
        [Route]
        public async Task<IHttpActionResult> Putusuario(string body) //int id, 
        {
            JObject o = JObject.Parse(body);
            string email = (string)o["email"];
            usuario Usuario = new usuario();

            try
            {
                Usuario.DPI = (string) o["dpi"];
                Usuario.Nombre = (string) o["nombre"];
                Usuario.Apellido = (string) o["apellido"];
                Usuario.telefono = (int) o["telefono"];
                Usuario.direccion = (string) o["direccion"];
                Usuario.fecha_nac = (DateTime) o["fechaNacimiento"];
                Usuario.correo = (string) o["email"];
                Usuario.username = (string) o["user"];
                Usuario.password = (string) o["password"];
                Usuario.tipo = (sbyte) o["tipo"];
                Usuario.estado = (sbyte) o["estado"];
            }
            catch (Exception e)
            {
                return BadRequest(ModelState);
            }


            if (email != Usuario.correo)
            {
                return BadRequest();
            }

            db.Entry(Usuario).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!usuarioExists(email))
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

        // POST: api/usuarios
        [ResponseType(typeof(usuario))]
        [Route]
        public async Task<IHttpActionResult> Postusuario(string body)
        {
            JObject o = JObject.Parse(body);
            usuario Usuario = new usuario();

            try
            {
                Usuario.DPI = (string)o["dpi"];
                Usuario.Nombre = (string)o["nombre"];
                Usuario.Apellido = (string)o["apellido"];
                Usuario.telefono = (int)o["telefono"];
                Usuario.direccion = (string)o["direccion"];
                //Usuario.fecha_nac = Convert.ToDateTime(o["fechaNacimiento"]);
                Usuario.fecha_nac = DateTime.Now;
                Usuario.correo = (string)o["email"];
                Usuario.username = (string)o["user"];
                Usuario.password = (string)o["password"];
                Usuario.tipo = (sbyte)o["tipo"];
                Usuario.estado = (sbyte)o["estado"];
            }
            catch (Exception e)
            {
                return BadRequest(ModelState);
            }

            db.usuarios.Add(Usuario);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = Usuario.idUSUARIO }, Usuario);
        }

        // DELETE: api/usuarios/5
        [ResponseType(typeof(usuario))]
        [Route]
        public async Task<IHttpActionResult> Deleteusuario(string email)
        {
            usuario usuario =  db.usuarios.FirstOrDefault(x => x.correo == email);
            if (usuario == null)
            {
                return NotFound();
            }

            db.usuarios.Remove(usuario);
            await db.SaveChangesAsync();

            return Ok(usuario);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool usuarioExists(string email)
        {
            return db.usuarios.Count(e => e.correo == email) > 0;
        }
    }
}