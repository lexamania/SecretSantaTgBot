namespace SecretSantaTgBot.Messages;

public class MessagesUA : MessagesBase
{
    public override string CommandError { get; } = "❌ Не роспізнана команда! ❌";
    public override string CommandBotMenu { get; } = "☃️ Меню Бота ☃️\n";
    public override string CommandHelp { get; } = "сторінка з усіма командами";
    public override string CommandParticipate { get; } = "взяти участь в Таємному Санті";
    public override string CommandStopParticipate { get; } = "прибрати себе зі списків Таємного Санти";
    public override string CommandShowParticipants { get; } = "показати список учасників";
    public override string CommandShowTarget { get; } = "показати мою ціль на Таємного Санту";
    public override string CommandShowTargetWishes { get; } = "показати бажання цілі";
    public override string CommandStartWishes { get; } = "розпочати додавати бажання";
    public override string CommandStopWishes { get; } = "зупинити додавати бажання";
    public override string CommandClearWishes { get; } = "очистити список своїх бажань";
    public override string CommandShowMyWishes { get; } = "показати мої бажання";

    public override string CommandCreateRoom { get; } = "створити нову кімнату";
    public override string CommandJoinRoom { get; } = "{room_id} доєднатися до існуючої кімнати";
    public override string CommandSelectRoom { get; } = "перейти до кімнати";
    public override string CommandDeleteRoom { get; } = "видалити кімнату";
    public override string CommandShowRooms { get; } = "показати список моїх кімнат";

    public override string ZeroRooms { get; } = "❌ В тебе немає кімнат! ❌";
    public override string RoomCreationEnterTitle { get; } = "Введіть назву кімнати:";
    public override string RoomCreationEnterDescription { get; } = "Введіть опис івенту:";
    public override string ChooseRoom { get; } = "Виберіть кімнату:";
    public override string EnterRealName { get; } = "Введіть ваше ім'я:";
    public override string RoomDoesntExist { get; } = "❌ Кімнати з таким кодом не знайдено❌ ";
    public override string RoomCreated { get; } = "🥳 Кімната створена! 🥳\n\nКод кімнати:";
    public override string RoomDeleted { get; } = "- кімнату видалено! 🥳";
    public override string RoomsList { get; } = "<b>Список твоїх кімнат</b>:";

    public override string UserNewParticipation { get; } = "🥳 Розпочинаємо реєстрацію, введіть ваше ім'я:";
    public override string EnteredNameError { get; } = "❌ Введене Ім'я не коректне. Спробуйте знову:";
    public override string UserParticipationEnd { get; } = "🥳 Вас зареєстровано! 🥳";
    public override string UserTakeParticipation { get; } = "💌 Ви вже приймаєте участь! 💌";
    public override string UserCantCancelParticipation { get; } = "Учасники вже були обрані, ви не можете піти на пів шляху🙂 Зверніться до Лекса🎅";
    public override string UserDontTakeParticipation { get; } = "🥶 Ви не приймаєте участь! 🥶";
    public override string UserRemovedFromParticipation { get; } = "🗿 Вас викреслено з участі! 🗿";
    public override string ParticipantsList { get; } = "🎅 Список учасників 🧑‍🎄";
    public override string EmptyParticipants { get; } = "🛎 Учасники ще не розпочали реєстрацію. Будь першим! 🛎";

    public override string UserTarget { get; } = "🎅 Ваша ціль";
    public override string TargetHaveZeroWishes { get; } = "🔕 Твоя ціль не вказала бажання 🔕";
    public override string UserHaveZeroWishes { get; } = "🔕 Ти ще не заповнив список бажань! 🔕";
    public override string UserWishesCleared { get; } = "Ваші побажання очищено❗️";
    public override string UserWishesList { get; } = "🎁 Список твоїх бажань 🎁";
    public override string TargetWishesList { get; } = "🎁 Список бажань твоєї цілі 🎁";
    public override string UserStartWishes { get; } = """
        🎁 <b>Ви можете розпочати відправляти ваші побажання.<B> 🎁

        <b><u>Правила<u><b>:
        1. Можете всі побажання відправити текстом за 1 повідомлення
        2. Можете відправити текст з прикріпленними картинками
        3. Чи просто самі картинки
        4. Коли закінчете відправте команду /stop_wishes
        """;
    public override string UserStopWishes { get; } = "🎁 Ваш список побажань збережено! 🥳";

    public override string SecretSantaStillOffline { get; } = "🛎 Розподілення Санти ще не розпочалося! 🛎";
    public override string StartSanta { get; } = "🎰 Розпочинаємо крутити барабан на Санту!.. 🎰";
}
