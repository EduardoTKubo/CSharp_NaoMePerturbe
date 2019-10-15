using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace NaoMePerturbe.Classes
{
    class clsEmail
    {

        public static async Task<string> EnviaEmailAsync(string _assunto ,string _msg)  
        {
            try
            {
                await Task.Run(() =>
                {
                    MailMessage e_mail = new MailMessage();

                    //e_mail.To.Add("eduardo.tkb@gmail.com");

                    e_mail.To.Add("flavio.alvarenga@rocketbit.com.br");
                    e_mail.To.Add("cassio.almeida@rocketbit.com.br");

                    e_mail.CC.Add("alfredo@tradecall.com.br");              // copia
                    e_mail.CC.Add("ricardo.carvalho@tradecall.com.br");

                    e_mail.Bcc.Add("eduardo.kubo@tradecall.com.br");        // copia oculta
                    e_mail.Bcc.Add("marcelo @tradecall.com.br");

                    //Attachment anexo = new Attachment(_arquivo);
                    //e_mail.Attachments.Add(anexo);

                    e_mail.Subject = _assunto;
                    e_mail.From = new MailAddress("eduardo.kubo@tradecall.com.br");
                    e_mail.IsBodyHtml = false;
                    e_mail.Body = _msg;
                    e_mail.Priority = MailPriority.High;

                    SmtpClient smtp = new SmtpClient("smtp.tradecall.com.br", 587);
                    //smtp.EnableSsl = true;
                    System.Net.NetworkCredential cred = new System.Net.NetworkCredential("eduardo.kubo@tradecall.com.br", "G5f5x5Kubo3");
                    smtp.Credentials = cred;
                    //smtp.UseDefaultCredentials = true;

                    smtp.Send(e_mail);
                });

                return "email enviado com sucesso";
            }
            catch (Exception e)
            {
                 return "erro ao enviar o email : " + e.Message;
            }
        }


    }
}
