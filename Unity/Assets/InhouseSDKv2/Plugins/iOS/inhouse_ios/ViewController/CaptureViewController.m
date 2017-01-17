//
//  CaptureViewController.m
//  Test
//
//  Created by Chau Giang on 9/1/16.
//  Copyright © 2016 Chau Giang. All rights reserved.
//

#import "CaptureViewController.h"

@interface CaptureViewController ()

@property (weak, nonatomic) IBOutlet UIView *inputNode;
@property (weak, nonatomic) IBOutlet UILabel *lblScore;


@end

@implementation CaptureViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view from its nib.
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

- (UIImage*) imageWithScore:(int)score {
    self.view;
    _lblScore.text = [NSString stringWithFormat:@"%d", score];
    UIImage* image = [self imageWithView:_inputNode];
    return image;
}

- (UIImage *) imageWithView:(UIView *)view
{
    UIGraphicsBeginImageContextWithOptions(view.bounds.size, view.opaque, 0.0);
    [view.layer renderInContext:UIGraphicsGetCurrentContext()];
    
    UIImage * img = UIGraphicsGetImageFromCurrentImageContext();
    
    UIGraphicsEndImageContext();
    
    return img;
}

@end
