import * as React from 'react';
import PropTypes from 'prop-types';
import Typography from '@mui/material/Typography';
import Grid from '@mui/material/Grid';
import Card from '@mui/material/Card';
import CardActionArea from '@mui/material/CardActionArea';
import CardContent from '@mui/material/CardContent';
import CardMedia from '@mui/material/CardMedia';
import Avatar from "@mui/material/Avatar";
import { useNavigate } from "react-router-dom";

function Message(props) {
    const { post } = props;
    let navigate = useNavigate();

    return (
        <Grid item xs={12} md={8} alignContent={"center"}>
            <Card sx={{display: 'flex'}}>
                <CardActionArea onClick= {() => {navigate(`${window.appConfig.API_URL}/message/timeline/${post.author}`)}} sx={{flex:1}}>
                    <Avatar sx={{width:56, height:56}} alt={"A"} src={post.avatar}/>
                </CardActionArea>
                <CardContent sx={{flex: 18}}>
                    <Typography component="h2" variant="h5">
                        {post.author}
                    </Typography>
                    <Typography variant="subtitle1" color="text.secondary">
                        {post.pubDate}
                    </Typography>
                    <Typography variant="subtitle1" paragraph>
                        {post.text}
                    </Typography>
                </CardContent>
            </Card>
        </Grid>
    );
}

Message.propTypes = {
    post: PropTypes.shape({
        pubDate: PropTypes.string.isRequired,
        Text: PropTypes.string.isRequired,
        avatar: PropTypes.string,
        Username: PropTypes.string.isRequired,
    }).isRequired,
};

export default Message;