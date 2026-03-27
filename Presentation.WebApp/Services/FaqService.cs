namespace Presentation.WebApp.Services;

public sealed record FaqItem
(
    int Id,
    string Title,
    string Content
)
{
    public static FaqItem Create(int id, string title, string content) => new(id, title, content);
}

public sealed record FaqResult
(
    bool Succeeded,
    IReadOnlyList<FaqItem>? Faqs = null,
    string? ErrorMessage = null
)
{
    public static FaqResult Ok(IReadOnlyList<FaqItem> faqs) => new(true, faqs);
    public static FaqResult Failed(string errorMessage) => new(false, null, errorMessage);
}

public interface IFaqService
{
    Task<FaqResult> GetFaqsAsync();
}

public class FaqService : IFaqService
{
    private List<FaqItem> faqItems =
        [
            FaqItem.Create(
                1,
                "Do I need prior gym experience to Join CoreFitness?",
                """
                <p>No, CoreFitness is designed for all fitness levels. Our trainers guide beginners with proper techniques and structured workout plans to help them start safely and confidently.</p>
                <ul>
                    <li>Every new member receives a personalized introduction to equipments</li>
                    <li>Workouts are designed to grow with you, ensuring beginners can start safely.</li>
                </ul>
                """
            ),

            FaqItem.Create(
                2,
                "What facilities are included with the membership?",
                """
                <p>Your CoreFitness membership includes access to a wide range of modern training facilities designed to support all fitness goals.</p>
                <ul>
                    <li>State-of-the-art gym equipment for strength and cardio training</li>
                    <li>Access to locker rooms, showers, and recovery areas</li>
                    <li>Optional add-ons such as personal training and specialized classes</li>
                </ul>
                """
            ),

            FaqItem.Create(
                3,
                "Can I try the gym before taking a membership?",
                """
                <p>No, CoreFitness is designed for all fitness levels. Our trainers guide beginners with proper techniques and structured workout plans to help them start safely and confidently.</p>
                <ul>
                    <li>Every new member receives a personalized introduction to equipments</li>
                    <li>Workouts are designed to grow with you, ensuring beginners can start safely.</li>
                </ul>
                """
            ),

            FaqItem.Create(
                4,
                "Are there group workout programs available?",
                """
                <p>Yes, we provide a variety of group workout programs suitable for different fitness levels and goals.</p>
                <ul>
                    <li>Classes include HIIT, strength training, yoga, and functional workouts</li>
                    <li>Led by certified trainers to ensure safe and effective sessions</li>
                    <li>Flexible schedule with multiple sessions throughout the week</li>
                </ul>
                """
            ),

            FaqItem.Create(
                5,
                "Is nutrition guidance included in the plans?",
                """
                <p>Yes, nutrition guidance can be included depending on your membership or training plan.</p>
                <ul>
                    <li>Basic nutritional advice to support your fitness goals</li>
                    <li>Personalized meal guidance available through coaching programs</li>
                    <li>Focus on sustainable habits and long-term results</li>
                </ul>
                """
            )
        ];

    public async Task<FaqResult> GetFaqsAsync()
    {
        return FaqResult.Ok(faqItems);
    }
}


