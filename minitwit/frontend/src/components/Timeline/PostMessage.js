//Component only visble if logged in
// Should call the backend's postMessage endpoint with the message in the textfield

// A wrapper for <Route> that redirects to the login
// screen if you're not yet authenticated.
import Button from "@mui/material/Button";
import * as React from "react";
import {TextField, ThemeProvider} from "@mui/material";
import {createTheme} from "@mui/material/styles";
import {useState} from "react";
import PropTypes from "prop-types";
import Box from '@mui/material/Box';

function PostMessage() {
    const [errorMessage, setErrorMessage] = useState("");
    const [message, setMessage] = useState("");
    const loggedInUser = localStorage.getItem("token"); // possible to see if user is logged in
    let handleSubmit = async (e) => {
        e.preventDefault();
        try {
            let res = await fetch(`${window.appConfig.API_URL}/message`, {
                method: "POST",
                body:{
                    User: loggedInUser,
                    message: message,
                },
            });
            let resJson = await res.json();
            if (res.status === 200) {
                setErrorMessage("Message posted successfully");
            } else {
                setErrorMessage("Some error occured");
            }
        } catch (err) {
            console.log(err);
        }
    };
    const theme = createTheme();

    function ShowTextField() {
        if (loggedInUser) {
            // SHOW THE TEXT FIELD THEY CAN WRITE IN
            return (
                <Box component="form" onSubmit={handleSubmit}>
                    <TextField
                        name={message}
                        type="text"
                        value={message}
                        required
                        id="message"
                        autoFocus
                        label="Message"
                        placeholder="What's on your mind?"
                        onChange={(e) => setMessage(e.target.value)}
                    />
                    <Button
                        type="submit"
                        variant="contained"
                        sx={{mt: 3, mb: 2}}
                    >
                        Post
                    </Button>
                    <div className="message">{errorMessage ? <p>{errorMessage}</p> : null}</div>
                </Box>
            )
        } else {
            return (<div><p>You need to be logged in to post a message</p></div>)
            // DON'T SHOW THE TEXT FIELD
        }
    }

    return (
        <React.Fragment>
            <ThemeProvider theme={theme}>
                <ShowTextField/>
            </ThemeProvider>
        </React.Fragment>
    )
}
export default PostMessage;



/*export default function postMessage() {

    /!*const formik = useFormik( {
    onSubmit: async (values) => {
        const messageToPost = {
            Message: values.message,
        }*!/
    const test =
        await postMessage(messageToPost)
            .then(response => {
                if (response.status !== 200) {
                    alert("Something went wrong. Could not login. Status: " + response.status)
                } else {
                    response.json().then(data => {
                        // we should do anything with the response, right?
                        navigate('/');
                    })
                }
            })
}
})
}*/

// need to return stuff
