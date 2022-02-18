using System.Data;
using System.Net.Mime;
using Microsoft.Data.Sqlite;
using MinitwitReact.Core;
using MinitwitReact.Entities;

namespace MinitwitReact;

public interface IMinitwit
{
    //"""Returns a new connection to the database."""
    SqliteConnection ConnectDb();
    
    //"""Creates the database tables."""
    void InitDb();
    
    IEnumerable<string> GetUsers();
    
    //EF core method: Gets all users
    Task<IEnumerable<UserDto>> GetAllUsers();

    User GetUser(long userid);
    // EF core method: Gets User by their id
    Task<UserDto> GetUserById(long userId);

    bool isFollow(long userid, string username);

    //"""Queries the database and returns a list of dictionaries."""
    ICollection<Dictionary<string, string>> QueryDb(SqliteCommand query, bool one=false);
    
    //"""Convenience method to look up the id for a username."""
    long GetUserId(string username);
    // returns userId based on their username
    Task<long> GetUserIdEf(string username);

    Task<UserDetailsDto?> GetUserDetialsById(long userid);

    //"""Format a timestamp for display."""
    DateTime FormatDatetime(string timestamp); //Can probably be done using datetime
    
    //"""Return the gravatar image for the given email address."""
    Uri gravatar_url(string email, int size = 80);

    
    //@app.before_request
    /*"""Make sure we are connected to the database each request and look
        up the current user so that we know he's there.
        """*/
    void before_request();
    
     //@app.after_request
    //"""Closes the database again at the end of the request."""
    void after_request(string response); //TODO response might be an enum


    //@app.route('/')
    /*"""Shows a users timeline or if no user is logged in it will
    redirect to the public timeline.  This timeline shows the user's
    messages as well as all the messages of followed users.
    """*/
    IEnumerable<(Message, User)> Timeline(long userid);
    
    //@app.route('/public')
    //"""Displays the latest messages of all users."""
    IEnumerable<(Message, User)> public_timeline();
    Task<IEnumerable<(MessageDto, UserDto)>> PublicTimelineEf();
    
    //@app.route('/<username>')
    //"""Display's a users tweets."""
    IEnumerable<(Message, User)> user_timeline(long sessionId ,string username);
    Task<IEnumerable<(MessageDto, UserDto)>> UserTimelineEf(long sessionId, string username);
    Task<bool> Follows(long sessionId, UserDto user);

    Task<IEnumerable<ValueTuple<MessageDto, UserDto>>> TimelineEf(long sessionId);
    
    //@app.route('/<username>/follow')
    //"""Adds the current user as follower of the given user."""
    long follow_user(string username, long userid);
    
    //@app.route('/<username>/unfollow')
     //"""Removes the current user as follower of the given user.""" 
     long unfollow_user(string username, long userid);
       
     //@app.route('/add_message', methods=['POST'])
     //"""Registers a new message for the user."""
     string add_message(long userid, string message);
         
     //@app.route('/login', methods=['GET', 'POST'])
     //"""Logs the user in."""
     long Login(string username, string pw);
     
     //@app.route('/register', methods=['GET', 'POST'])
     //"""Registers the user."""
     long Register(string username, string email, string pw);
         
     //@app.route('/logout')
     //"""Logs the user out"""
     void Logout(long userid);        
}