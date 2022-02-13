using System.Data;
using Microsoft.Data.Sqlite;

namespace MinitwitReact;

public interface IMinitwit
{
    //"""Returns a new connection to the database."""
    SqliteConnection ConnectDb();
    
    //"""Creates the database tables."""
    void InitDb();

    IEnumerable<string> GetUsers();

    DataTable GetSchema();
    
    //"""Queries the database and returns a list of dictionaries."""
    void QueryDb( /*query, args=(), one=False*/);
    
    //"""Convenience method to look up the id for a username."""
    void GetUserId(string username);
    
    //"""Format a timestamp for display."""
    void FormatDatetime(string timestamp); //Can probably be done using datetime
    
    //"""Return the gravatar image for the given email address."""
    void gravatar_url(string email, int size = 80);
    
    //@app.before_request
    /*"""Make sure we are connected to the database each request and look
        up the current user so that we know he's there.
        """*/
    void before_request();
    
     //@app.after_request
    //"""Closes the database again at the end of the request."""
    void after_request(string response); //TODO response might be and enum

    //@app.route('/')
    /*"""Shows a users timeline or if no user is logged in it will
    redirect to the public timeline.  This timeline shows the user's
    messages as well as all the messages of followed users.
    """*/
    void Timeline();
    
    //@app.route('/public')
    //"""Displays the latest messages of all users."""
    void public_timeline();
    
    //@app.route('/<username>')
    //"""Display's a users tweets."""
    void user_timeline(string username);

    
    //@app.route('/<username>/follow')
    //"""Adds the current user as follower of the given user."""
    void follow_user(string username);
    
    //@app.route('/<username>/unfollow')
     //"""Removes the current user as follower of the given user.""" 
     void unfollow_user(string username);
       
     //@app.route('/add_message', methods=['POST'])
     //"""Registers a new message for the user."""
     void add_message();
         
     //@app.route('/login', methods=['GET', 'POST'])
     //"""Logs the user in."""
     void Login();
     
     //@app.route('/register', methods=['GET', 'POST'])
     //"""Registers the user."""
     void Register();
         
     //@app.route('/logout')
     //"""Logs the user out"""
     void Logout();
         
     
}