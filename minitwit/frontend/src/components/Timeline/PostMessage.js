//Component only visble if logged in
// backend's postMessage endpoint with the message in the textfield

import Button from "@mui/material/Button";
import * as React from "react";
import {TextField, ThemeProvider} from "@mui/material";
import {createTheme} from "@mui/material/styles";
import {useState} from "react";
import Box from '@mui/material/Box';

function PostMessage() {
    const [errorMessage, setErrorMessage] = useState("");
    const [message, setMessage] = useState("");
    const loggedInUser = localStorage.getItem("token"); // possible to see if user is logged in
    let trimmed = ""
    if(loggedInUser) {
        trimmed = loggedInUser.substring(1, loggedInUser.length - 1);
    }
    let handleSubmit = async (e) => {
        e.preventDefault();
            var myHeaders = new Headers();
            myHeaders.append("Authorization", "Bearer " + trimmed);
            myHeaders.append("Content-Type", "application/json");

            var raw = JSON.stringify({
                Text: message,
            });
            
            var requestOptions = {
                method: 'POST',
                headers: myHeaders,
                body: raw,
                redirect: 'follow'
            };

            fetch(`${window.appConfig.API_URL}/message`, requestOptions)
                .then(response => response.text())
                .then(result => console.log(result))
                .catch(error => console.log('error', error));
    };
    const theme = createTheme();

    function ShowTextField() {
        if (loggedInUser) {
            // SHOW THE TEXT FIELD THEY CAN WRITE IN
            return (
                <Box component="form" onSubmit={handleSubmit} style={{display: 'flex',  justifyContent:'center'}}>
                    <TextField
                        name={message}
                        type="text"
                        value={message}
                        id="message"
                        autoFocus
                        label="Message"
                        placeholder="What's on your mind?"
                        onChange={(e) => setMessage(e.target.value)}
                    />
                    <Button
                        type="submit"
                        variant="contained"
                        sx={
                            {
                                mt: 2.3,
                                mb: 2,
                                ml: 2
                            }
                    }
                    >
                        Post
                    </Button>
                    <div className="message">{errorMessage ? <p>{errorMessage}</p> : null}</div>
                </Box>
            )
        } else {
            return (<div style={{display: 'flex',  justifyContent:'center'}}><p><i>You need to be logged in to post a message</i></p></div>)
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
