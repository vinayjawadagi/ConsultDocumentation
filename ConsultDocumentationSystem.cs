using System;
using System.Collections.Generic;
using System.Linq;

// Simple Consult documentation system
namespace ConsultDocumentationSystem
{
    // Consultation class
    public class Consultation
    {
        public int Id { get; set; }
        public string PatientId { get; set; }
        public string PhysicianId { get; set; }
        public DateTime ConsultDate { get; set; }
        public string Complaint { get; set; }
        public string Assessment { get; set; }
        public string Plan { get; set; }
        public ConsultationType Type { get; set; }
        public ConsultationStatus Status { get; set; }
    }

    public enum ConsultationType
    {
        Initial,
        FollowUp,
        Urgent,
        Routine
    }

    public enum ConsultationStatus
    {
        Scheduled,
        InProgress,
        Completed,
        Cancelled
    }

    public class ConsultationService
    {
        private List<Consultation> _consultations;

        public ConsultationService()
        {
            _consultations = new List<Consultation>();
        }

        public int CreateConsultation(string patientId, string physicianId, 
            ConsultationType type, string complaint)
        {
            var consultation = new Consultation
            {
                Id = GenerateConsultId(),
                PatientId = patientId,
                PhysicianId = physicianId,
                ConsultDate = DateTime.Now,
                Complaint = complaint,
                Type = type,
                Status = ConsultationStatus.Scheduled // Default value
            };

            _consultations.Add(consultation);
            return consultation.Id;
        }

        public bool UpdateConsultation(int consultId, string assessment, string plan)
        {
            var consult = _consultations.FirstOrDefault(c => c.Id == consultId);
            
            // Return false if consultID doesnt exist
            if (consult == null)
            {
                return false;
            }
            
            consult.Assessment = assessment;
            consult.Plan = plan;
            consult.Status = ConsultationStatus.Completed;
    
            return true;
        }

        public List<Consultation> GetPhysicianConsultations(string physicianId)
        {
            return _consultations
                .Where(c => c.PhysicianId == physicianId)
                .OrderByDescending(c => c.ConsultDate)
                .ToList();
        }

        public List<Consultation> GetPatientConsultations(string patientId)
        {
            return _consultations
                .Where(c => c.PatientId == patientId)
                .OrderByDescending(c => c.ConsultDate)
                .ToList();
        }

        private int GenerateConsultId()
        {
            // This makes ConsultIds more readable, but depending on the scale of users can be changed.
            // Start generating sequentially consultIds from 1
            return _consultations.Count > 0 ? 
                _consultations.Max(c => c.Id) + 1 : 1;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var consultService = new ConsultationService();

            // Create consultations
            int consultId1 = consultService.CreateConsultation(
                patientId: "P1",
                physicianId: "DR1",
                type: ConsultationType.Initial,
                complaint: "headache for 3 days"
            );

            int consultId2 = consultService.CreateConsultation(
                patientId: "P1",
                physicianId: "DR1",
                type: ConsultationType.FollowUp,
                complaint: "follow up for headache"
            );

            Console.WriteLine($"Created consultation IDs: {consultId1}, {consultId2}");

            
            // Update consultation
            consultService.UpdateConsultation(
                consultId1,
                assessment: "Probably dehydrated",
                plan: "Drink a lot of water."
            );


            // Get all consults by Physician
            var doctorConsults = consultService.GetPhysicianConsultations("DR1");
            
            Console.WriteLine($"Fetching Colnsultations for DR1, found {doctorConsults.Count}");
            foreach (var consult in doctorConsults)
            {
                Console.WriteLine($"\nConsultation ID: {consult.Id} Patient ID: {consult.PatientId} Type: {consult.Type}");
                Console.WriteLine($"Status: {consult.Status} Complaint: {consult.Complaint}");
                // Only print assesment and plan if updated by the doc
                if (!string.IsNullOrEmpty(consult.Assessment))
                {
                    Console.WriteLine($"Assessment: {consult.Assessment}");
                    Console.WriteLine($"Plan: {consult.Plan}");
                }
            }
            
        }
    }
    
}
    
