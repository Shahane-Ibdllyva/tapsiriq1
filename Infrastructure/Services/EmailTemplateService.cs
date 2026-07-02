using Application.Interfaces; // <-- Interfeysi daxil edirik
using System.Collections.Generic;

namespace Infrastructure.Services
{
    // Interfeysi implement edirik
    public class EmailTemplateService : IEmailTemplateService
    {
        public string GetWelcomeEmail(string fullName, string username)
        {
            return $@"
                <h1>Xoş gəldiniz, {fullName}!</h1>
                <p>İstifadəçi adınız: <b>{username}</b></p>
                <p>Hesabınızı aktivləşdirmək üçün <a href='#'>buraya</a> klikləyin.</p>
            ";
        }

        public string GetPasswordResetEmail(string fullName, string token)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 8px; }}
                        .header {{ background: #4a90e2; color: white; padding: 15px; border-radius: 8px 8px 0 0; }}
                        .code-box {{ background: #f5f7fa; padding: 25px; border-radius: 8px; text-align: center; font-size: 36px; letter-spacing: 10px; font-weight: bold; color: #2c3e50; border: 2px dashed #4a90e2; margin: 20px 0; }}
                        .footer {{ margin-top: 20px; font-size: 12px; color: #999; border-top: 1px solid #eee; padding-top: 10px; }}
                        .warning {{ color: #e74c3c; font-weight: bold; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h2>🔐 Şifrə Sıfırlama</h2>
                        </div>
                        
                        <p>Salam, <b>{fullName}</b>!</p>
                        
                        <p>Şifrənizi sıfırlamaq üçün aşağıdakı <b>6 rəqəmli təsdiq kodundan</b> istifadə edin:</p>
                        
                        <div class='code-box'>
                            {token}
                        </div>
                        
                        <p>
                            ⏱️ Bu kod <span class='warning'>5 dəqiqə</span> ərzində etibarlıdır.
                            <br />
                            Kod etibarlılıq müddəti bitərsə, yeni kod istəməlisiniz.
                        </p>
                        
                        <p>
                            Təhlükəsizlik üçün bu kodu <b>heç kimlə paylaşmayın</b>.
                        </p>
                        
                        <div class='footer'>
                            <p>
                                Əgər bu əməliyyatı siz etməmisinizsə, bu məktubu nəzərə almayın.
                                <br />
                                Bu avtomatik yaradılmış mesajdır, cavab verməyin.
                            </p>
                        </div>
                    </div>
                </body>
                </html>
            ";
        }
    }
}