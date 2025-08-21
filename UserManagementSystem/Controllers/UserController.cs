using Microsoft.AspNetCore.Mvc;
using UserManagementSystem.Data;
using UserManagementSystem.Models;
using Microsoft.AspNetCore.Http;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;



namespace UserManagementSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Login()
        {
            var model = new LoginViewModel();
            if (TempData["Input_UserName"] != null)
            {
                model.UserName = TempData["Input_UserName"].ToString();
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel loginViewModels)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var u = _context.Users.FirstOrDefault(x => x.UserName == loginViewModels.UserName && x.Password == loginViewModels.Password);

                    if (u != null)
                    {
                        HttpContext.Session.SetString("UserName", u.UserName);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["PopupMessage"] = "Invalid Username and Password";
                        TempData["Input_UserName"] = loginViewModels.UserName;
                        TempData["Input_Password"] = loginViewModels.Password;
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["PopupMessage"] = "An error occurred: " + ex.Message;
            }

            return RedirectToAction("Login");
        }

        //public IActionResult Signup()
        //{
        //    var model = new SignupViewModel
        //    {
        //        NewUser = new Users(),
        //        UserList = _context.Users.OrderByDescending(u => u.Id).Take(10).ToList()
        //    };
        //    return View(model);
        //}

        [HttpGet]
        public IActionResult Signup()
        {
            var model = new SignupViewModel
            {
                NewUser = new Users(),
                UserList = _context.Users.OrderByDescending(u => u.Id).Take(10).ToList()
            };

            if (TempData.ContainsKey("FormData"))
            {
                var formDataJson = TempData["FormData"]?.ToString();
                if (!string.IsNullOrEmpty(formDataJson))
                {
                    model.NewUser = JsonSerializer.Deserialize<Users>(formDataJson);
                }
            }
            return View(model);
        }

        //[HttpPost]
        //public async Task<IActionResult> Signup(SignupViewModel model)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            model.UserList = _context.Users.OrderByDescending(u => u.Id).Take(10).ToList();
        //            return View(model);
        //        }

        //        if (model.NewUser.Id > 0)
        //        {
        //            var existingUser = await _context.Users.FindAsync(model.NewUser.Id);
        //            if (existingUser == null) return NotFound();

        //            existingUser.UserName = model.NewUser.UserName;
        //            existingUser.Password = model.NewUser.Password;
        //            existingUser.ConfirmPassword = model.NewUser.ConfirmPassword;
        //            existingUser.Email = model.NewUser.Email;
        //            existingUser.ContactNo = model.NewUser.ContactNo;
        //            existingUser.NRCNo = model.NewUser.NRCNo;

        //            await _context.SaveChangesAsync();
        //            ModelState.Clear();
        //            TempData["PopupMessage"] = "User successfully updated!";
        //        }
        //        else
        //        {
        //            bool userExists = _context.Users.Any(u => u.UserName == model.NewUser.UserName);
        //            if (userExists)
        //            {
        //                TempData["PopupMessage"] = "Username already exists.";
        //                model.UserList = _context.Users.OrderByDescending(u => u.Id).Take(10).ToList();
        //            }

        //            bool emailExists = _context.Users.Any(u => u.Email == model.NewUser.Email);
        //            if (emailExists)
        //            {
        //                TempData["PopupMessage"] = "Email already registered.";
        //                model.UserList = _context.Users.OrderByDescending(u => u.Id).Take(10).ToList();
        //                return RedirectToAction("Signup");
        //            }

        //            _context.Users.Add(model.NewUser);
        //            await _context.SaveChangesAsync();
        //            ModelState.Clear();
        //            TempData["PopupMessage"] = "User successfully registered!";
        //        }

        //        var newModel = new SignupViewModel
        //        {
        //            NewUser = new Users(),
        //            UserList = _context.Users.OrderByDescending(u => u.Id).Take(10).ToList()
        //        };

        //        return RedirectToAction("Signup");
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["PopupMessage"] = "An error occurred: " + ex.Message;
        //        model.UserList = _context.Users.OrderByDescending(u => u.Id).Take(10).ToList();
        //        return RedirectToAction("Signup");
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> Signup(SignupViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.UserList = _context.Users.OrderByDescending(u => u.Id).Take(10).ToList();
                    return View(model);
                }

                // For Update
                if (model.NewUser.Id > 0)
                {
                    var existingUser = await _context.Users.FindAsync(model.NewUser.Id);
                    if (existingUser == null) return NotFound();

                    existingUser.UserName = model.NewUser.UserName;
                    existingUser.Password = model.NewUser.Password;
                    existingUser.ConfirmPassword = model.NewUser.ConfirmPassword;
                    existingUser.Email = model.NewUser.Email;
                    existingUser.ContactNo = model.NewUser.ContactNo;
                    existingUser.NRCNo = model.NewUser.NRCNo;

                    await _context.SaveChangesAsync();
                    TempData["PopupMessage"] = "User successfully updated!";
                }
                else
                {
                    if (_context.Users.Any(u => u.UserName == model.NewUser.UserName))
                    {
                        TempData["PopupMessage"] = "Username already exists.";
                        TempData["FormData"] = JsonSerializer.Serialize(model.NewUser);
                        return RedirectToAction("Signup");
                    }
                    if (_context.Users.Any(u => u.Email == model.NewUser.Email))
                    {
                        TempData["PopupMessage"] = "Email already registered.";
                        TempData["FormData"] = JsonSerializer.Serialize(model.NewUser);
                        return RedirectToAction("Signup");
                    }

                    _context.Users.Add(model.NewUser);
                    await _context.SaveChangesAsync();
                    TempData["PopupMessage"] = "User successfully registered!";
                }

                return RedirectToAction("Signup");
            }
            catch (Exception ex)
            {
                TempData["PopupMessage"] = "An error occurred: " + ex.Message;
                TempData["FormData"] = JsonSerializer.Serialize(model.NewUser);
                return RedirectToAction("Signup");
            }
        }


        [HttpGet]
        public IActionResult ResetPassword(string email = null)
        {
            var model = new ResetPasswordModel();
            if (TempData["Input_Email"] != null)
            {
                model.Email = TempData["Input_Email"].ToString();
            }
            else if (!string.IsNullOrEmpty(email))
            {
                model.Email = email;
            }
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            if (!ModelState.IsValid)
            {
                return View("ResetPassword");
            }

            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == resetPasswordModel.Email);
                if (user != null)
                {
                    user.Password = resetPasswordModel.NewPassword;
                    await _context.SaveChangesAsync();

                    TempData["PopupMessage"] = "Password has been reset successfully!";
                }
                else
                {
                    TempData["PopupMessage"] = "Email not found.";
                    TempData["Input_Email"] = resetPasswordModel.Email;
                }
            }
            catch (Exception ex)
            {
                TempData["PopupMessage"] = "An error occurred: " + ex.Message;
                TempData["Input_Email"] = resetPasswordModel.Email;
            }

            return RedirectToAction("ResetPassword");
        }



        [HttpGet]
        public IActionResult Edit(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new SignupViewModel
            {
                NewUser = user,
                UserList = _context.Users.OrderByDescending(u => u.Id).Take(10).ToList()
            };

            return View("Signup", model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                {
                    TempData["PopupMessage"] = "User not found.";
                }
                else
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                    TempData["PopupMessage"] = "User successfully deleted!";
                }
            }
            catch (Exception ex)
            {
                TempData["PopupMessage"] = "An error occurred: " + ex.Message;
            }
            var newModel = new SignupViewModel
            {
                NewUser = new Users(),
                UserList = _context.Users.OrderByDescending(u => u.Id).Take(10).ToList()
            };

            return View("Signup", newModel);
        }

        public IActionResult GenerateCaptcha()
        {
            var randomText = new Random().Next(1000, 9999).ToString();
            HttpContext.Session.SetString("CaptchaCode", randomText);

            var bitmap = new Bitmap(100, 40);
            var graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);
            graphics.DrawString(randomText, new Font("Arial", 20), Brushes.Black, new PointF(10, 5));

            var stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream.ToArray(), "image/png");
        }

    }
}
